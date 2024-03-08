using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using MassTransit;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ComandoManager : IComandoManager
    {
        private readonly IComandoRepository _comandoRepository;
        private readonly IErroManager _erroManager;
        private readonly IBucketManager _bucketManager;
        private readonly IOpenAIVendor _openAIVendor;
        private readonly IBus _bus;

        public ComandoManager(IComandoRepository comandoRepository,
                              IErroManager erroManager,
                              IBucketManager bucketManager,
                              IOpenAIVendor openAIVendor,
                              IBus bus)
        {
            _comandoRepository = comandoRepository;
            _erroManager = erroManager;
            _bucketManager = bucketManager;
            _openAIVendor = openAIVendor;
            _bus = bus;
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
                }

                await ProcessarComando(comando);
            }
            catch (Exception ex)
            {
                novoProcessamentoComando.MensagemErro = ex.Message;

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
                InstanteCriacao = DateTime.Now
            };

            comando.ProcessamentosComandos.Add(novoProcessamentoComando);

            return novoProcessamentoComando;
        }

        private async Task SalvarAudio(Comando comando)
        {
            var bytes = comando.Payload;
            var urlAudio = await _bucketManager.ArmazenarAudio(bytes);
            comando.UrlAudio = urlAudio;
        }

        private async Task GerarTexto(Comando comando)
        {
        }
    }
}