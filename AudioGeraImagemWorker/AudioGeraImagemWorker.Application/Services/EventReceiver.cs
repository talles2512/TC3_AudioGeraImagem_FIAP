using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
