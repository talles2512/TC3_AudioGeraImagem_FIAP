using AudioGeraImagemWorker.Domain.Entities;

namespace AudioGeraImagemWorker.Domain.Interfaces
{
    public interface IComandoManager
    {
        Task ProcessarComando(Comando comando);
    }
}