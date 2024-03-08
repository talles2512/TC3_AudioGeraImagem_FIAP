using AudioGeraImagemWorker.Domain.Entities;

namespace AudioGeraImagemWorker.Application.Interfaces
{
    public interface IEventReceiver
    {
        Task ReceberEvento(Comando comando);
    }
}