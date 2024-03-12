using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities.Request;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Factories;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
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

        private readonly IConfiguration _configuration;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public ComandoManager(IComandoRepository comandoRepository,
                              IErroManager erroManager,
                              IBucketManager bucketManager,
                              IOpenAIVendor openAIVendor,
                              IConfiguration configuration,
                              ILogger<ComandoManager> logger)
        {
            _comandoRepository = comandoRepository;
            _erroManager = erroManager;
            _bucketManager = bucketManager;
            _openAIVendor = openAIVendor;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ProcessarComando(Comando comando)
        {
            await AtualizarProcessamentoComando(comando);
            await ExecutarComando(comando);
        }

        public async Task ExecutarComando(Comando comando)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            try
            {
                switch (ultimoProcessamento.Estado)
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
                        return;
                }

                await AtualizarProcessamentoComando(comando);
                await ExecutarComando(comando);
            }
            catch (Exception ex)
            {
                ultimoProcessamento.MensagemErro = ex.Message;
                await _erroManager.TratarErro(comando, ultimoProcessamento.Estado);
            }
        }

        private async Task AtualizarProcessamentoComando(Comando comando)
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

            comando.InstanteAtualizacao = DateTime.Now;

            var novoProcessamentoComando = ProcessamentoComandoFactory.Novo(novoEstadoComando);

            comando.ProcessamentosComandos.Add(novoProcessamentoComando);

            await _comandoRepository.Atualizar(comando);
        }

        #region [ Tratamentos dos Estados dos Comandos ]

        // 1. Estado Recebido >> Salvando Audio
        private async Task SalvarAudio(Comando comando)
        {
            try
            {
                var bytes = comando.Payload;
                var urlAudio = await _bucketManager.ArmazenarObjeto(bytes, string.Concat(comando.Id.ToString(), ".mp3"));
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }

        // 5. Gerando Imagem >> Salvado Imagem
        private async Task SalvarImagem(Comando comando)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(comando.UrlAudio);

                var urlImagem = await _bucketManager.ArmazenarObjeto(bytes, string.Concat(comando.Id.ToString(), ".jpeg"));
                comando.UrlImagem = urlImagem;

                await _comandoRepository.Atualizar(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarImagem] => Exception.: {ex.Message}");
                throw;
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
                throw;
            }
        }

        #endregion
    }
}