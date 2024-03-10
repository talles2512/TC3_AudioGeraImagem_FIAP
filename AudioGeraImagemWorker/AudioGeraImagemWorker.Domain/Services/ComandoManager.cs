using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities.Request;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ComandoManager : IComandoManager
    {
        private readonly IComandoRepository _comandoRepository;
        private readonly IErroManager _erroManager;
        private readonly IBucketManager _bucketManager;
        private readonly IOpenAIVendor _openAIVendor;
        private readonly IBus _bus;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public ComandoManager(IComandoRepository comandoRepository,
                              IErroManager erroManager,
                              IBucketManager bucketManager,
                              IOpenAIVendor openAIVendor,
                              IBus bus,
                              IConfiguration configuration,
                              ILogger<ComandoManager> logger)
        {
            _comandoRepository = comandoRepository;
            _erroManager = erroManager;
            _bucketManager = bucketManager;
            _openAIVendor = openAIVendor;
            _bus = bus;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ProcessarComando(Comando comando)
        {
            var novoProcessamentoComando = await AtualizarProcessamentoComando(comando);

            try
            {
                switch (novoProcessamentoComando.Estado)
                {
                    case EstadoComando.SalvandoAudio:
                        await SalvarAudio(comando);
                        break;

                    case EstadoComando.GerandoTexto:
                        await GerarTexto(comando);
                        break;

                    case EstadoComando.SalvandoTexto:
                        await SalvarTexto(comando);
                        break;

                    case EstadoComando.GerandoImagem:
                        await GerarImagem(comando);
                        break;

                    case EstadoComando.SalvadoImagem:
                        await SalvarImagem(comando);
                        break;

                    case EstadoComando.Finalizado:
                        await Finalizar(comando);
                        break;
                }

                await ProcessarComando(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [ProcessarComando] => Exception.: {ex.Message}");
                novoProcessamentoComando.MensagemErro = ex.Message;
                comando.ProcessamentosComandos.Add(novoProcessamentoComando);
                await _erroManager.TratarErro(comando);
            }
        }

        private async Task<ProcessamentoComando> AtualizarProcessamentoComando(Comando comando)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            EstadoComando novoEstadoComando = default;

            switch (ultimoProcessamento.Estado)
            {
                case EstadoComando.Recebido:
                    novoEstadoComando = EstadoComando.SalvandoAudio;
                    break;

                case EstadoComando.SalvandoAudio:
                    novoEstadoComando = EstadoComando.GerandoTexto;
                    break;

                case EstadoComando.GerandoTexto:
                    novoEstadoComando = EstadoComando.SalvandoTexto;
                    break;

                case EstadoComando.SalvandoTexto:
                    novoEstadoComando = EstadoComando.GerandoImagem;
                    break;

                case EstadoComando.GerandoImagem:
                    novoEstadoComando = EstadoComando.SalvadoImagem;
                    break;

                case EstadoComando.SalvadoImagem:
                    novoEstadoComando = EstadoComando.Finalizado;
                    break;
            }

            var novoProcessamentoComando = CriarNovoProcessamento(comando, novoEstadoComando);

            await _comandoRepository.Atualizar(comando);

            return novoProcessamentoComando;
        }

        private ProcessamentoComando CriarNovoProcessamento(Comando comando, EstadoComando novoEstado)
        {
            comando.InstanteAtualizacao = DateTime.Now;

            var novoProcessamentoComando = new ProcessamentoComando()
            {
                Estado = novoEstado,
                InstanteCriacao = DateTime.Now,
                Tentativa = 0 //Toda vez que receber um novo comando zeramos a tentativa.
            };

            comando.ProcessamentosComandos.Add(novoProcessamentoComando);

            return novoProcessamentoComando;
        }

        #region [ Tratamentos dos Estados dos Comandos ]

        // 1. Estado Recebido >> Salvando Audio
        private async Task SalvarAudio(Comando comando)
        {
            try
            {
                var bytes = comando.Payload;
                var urlAudio = await _bucketManager.ArmazenarAudio(bytes);
                if (string.IsNullOrEmpty(urlAudio))
                {
                    _logger.LogError($"[{_className}] - [SalvarAudio] => Exception.: Falha no armazenamento do audio no Azure Blob Storage");
                }
                else
                {
                    comando.UrlAudio = urlAudio;
                    //Salvando URL do Blob do Audio
                    await _comandoRepository.Atualizar(comando);
                }   
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarAudio] => Exception.: {ex.Message}");
            }   
        }

        // 2. Salvando Audio >> Gerando Texto
        private async Task GerarTexto(Comando comando)
        {
            try
            {
                TranscricaoRequest request = new TranscricaoRequest();
                request.model = _configuration.GetSection("OpenAI:TranscriptionConfiguration")["model"] ?? string.Empty;
                request.bytes = comando.Payload;
                request.filename = "audio.mp3";

                var result = await _openAIVendor.Transcricao(request);

                if (result.Item1 != null && string.IsNullOrEmpty(result.Item2))
                {
                    comando.TextoProcessado = result.Item1.text;
                }
                else
                {
                    _logger.LogError($"[{_className}] - [GerarTexto] => Exception.: {result.Item2}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarTexto] => Exception.: {ex.Message}");
            }
        }

        // 3. Gerando Texto >> Salvando Texto
        private async Task SalvarTexto(Comando comando)
        {
            try
            {
                await _comandoRepository.Atualizar(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarTexto] => Exception.: {ex.Message}");
            }
        }

        // 4. Salvando Texto >> Gerando Imagem
        private async Task GerarImagem(Comando comando)
        {
            try
            {
                GerarImagemRequest request = new GerarImagemRequest();
                request.model = _configuration.GetSection("OpenAI:GenerateImageConfiguration")["model"] ?? string.Empty;
                request.n = int.Parse(_configuration.GetSection("OpenAI:GenerateImageConfiguration")["n"]);
                request.size = _configuration.GetSection("OpenAI:GenerateImageConfiguration")["size"] ?? string.Empty;

                var result = await _openAIVendor.GerarImagem(request);
                if (result.Item1 != null && string.IsNullOrEmpty(result.Item2))
                {
                    comando.UrlImagem = result.Item1.data[0].url;
                }
                else
                {
                    _logger.LogError($"[{_className}] - [GerarImagem] => Exception.: {result.Item2}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarImagem] => Exception.: {ex.Message}");
            }
        }

        // 5. Gerando Imagem >> Salvado Imagem
        private async Task SalvarImagem(Comando comando)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(comando.UrlImagem);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Falha no download da imagem da URL do Blob. Status code: {response.StatusCode}");
                }

                using MemoryStream memoryStream = new MemoryStream();
                await response.Content.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                var urlImagem = await _bucketManager.ArmazenarImagem(bytes);
                if (string.IsNullOrEmpty(urlImagem))
                {
                    _logger.LogError($"[{_className}] - [SalvarImagem] => Exception.: Falha no armazenamento da imagem no Azure Blob Storage");
                }
                else
                {
                    comando.UrlImagem = urlImagem;
                    //Salvando URL do Blob do Audio
                    await _comandoRepository.Atualizar(comando);
                }



                await _comandoRepository.Atualizar(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarImagem] => Exception.: {ex.Message}");
            }
        }

        // 6. Salvando Imagem >> Finalizado
        private async Task Finalizar(Comando comando)
        {
            try
            {
                await _comandoRepository.Atualizar(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Finalizar] => Exception.: {ex.Message}");
            }
        }

        #endregion
    }
}