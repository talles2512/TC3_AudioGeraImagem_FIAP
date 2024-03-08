using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ErroManager : IErroManager
    {
        public async Task TratarErro(Comando comando)
        {
            await Task.Delay(1000); //apenas teste
            throw new NotImplementedException();
        }
    }
}