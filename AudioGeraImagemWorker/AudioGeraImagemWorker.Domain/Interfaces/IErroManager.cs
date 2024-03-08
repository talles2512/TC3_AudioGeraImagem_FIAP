using AudioGeraImagemWorker.Domain.Entities;

namespace AudioGeraImagemWorker.Domain.Interfaces
{
    public interface IErroManager
    {
        Task TratarErro(Comando comando);
    }
}