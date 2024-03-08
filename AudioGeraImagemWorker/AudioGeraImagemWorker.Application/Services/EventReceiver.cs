using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces;

namespace AudioGeraImagemWorker.Application.Services
{
    public class EventReceiver : IEventReceiver
    {
        private readonly IComandoManager _comandoManager;

        public EventReceiver(IComandoManager comandoManager)
        {
            _comandoManager = comandoManager;
        }

        public async Task ReceberEvento(Comando comando)
        {
            await _comandoManager.ProcessarComando(comando);
        }
    }
}