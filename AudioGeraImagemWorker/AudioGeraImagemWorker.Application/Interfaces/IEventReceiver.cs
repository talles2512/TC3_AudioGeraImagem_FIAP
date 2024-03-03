using AudioGeraImagemWorker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Application.Interfaces
{
    public interface IEventReceiver
    {
        Task ReceberEvento(Comando comando);
    }
}
