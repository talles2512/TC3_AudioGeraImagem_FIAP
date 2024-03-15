using AudioGeraImagem.Domain.Entities;
using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Factories;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using MassTransit;
using MassTransit.Scheduling;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ErroManager : IErroManager
    {
        private readonly IComandoRepository _comandoRepository;
        private readonly IBus _bus;

        public ErroManager(
            IComandoRepository comandoRepository,
            IBus bus)
        {
            _comandoRepository = comandoRepository;
            _bus = bus;
        }

        public async Task TratarErro(Comando comando, EstadoComando ultimoEstado, byte[] payload)
        {
            comando.InstanteAtualizacao = DateTime.Now;

            var ultimosProcessamentos = comando.ProcessamentosComandos.Where(x => x.Estado == ultimoEstado);

            ProcessamentoComando novoProcessamentoComando = null;

            if (ultimosProcessamentos.Count() < 3)
                novoProcessamentoComando = ProcessamentoComandoFactory.Novo(ultimoEstado);
            else
                novoProcessamentoComando = ProcessamentoComandoFactory.Novo(EstadoComando.Falha);

            comando.ProcessamentosComandos.Add(novoProcessamentoComando);
            await _comandoRepository.Atualizar(comando);

            if (novoProcessamentoComando.Estado != EstadoComando.Falha)
            {
                var mensagem = CriarMensagem(comando, payload);
                await PublicarMensagem(mensagem);
            }
        }

        private ComandoMessage CriarMensagem(Comando comando, byte[] payload)
        {
            return new()
            {
                ComandoId = comando.Id,
                Payload = payload
            };
        }
        private async Task PublicarMensagem(ComandoMessage mensagem)
        {
            var messageScheduler = _bus.CreateMessageScheduler(new Uri("queue:retentativa"));
            await messageScheduler.SchedulePublish(DateTime.UtcNow.AddSeconds(30), mensagem);
        }
    }
}