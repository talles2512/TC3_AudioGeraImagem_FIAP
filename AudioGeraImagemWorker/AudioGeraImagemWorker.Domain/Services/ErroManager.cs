using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Factories;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using MassTransit;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ErroManager : IErroManager
    {
        private readonly IComandoRepository _comandoRepository;
        private readonly IBus _bus;

        public ErroManager(IComandoRepository comandoRepository, IBus bus)
        {
            _comandoRepository = comandoRepository;
            _bus = bus;
        }

        public async Task TratarErro(Comando comando, EstadoComando ultimoEstado)
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

            // Envia para uma fila de reprocessamento caso não tenha estourado o limite de falhas
            if (novoProcessamentoComando.Estado != EstadoComando.Falha)
                await _bus.Send(comando);

        }
    }
}