using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Domain.Entities;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Worker.Events
{
    public class ComandoConsumer : IConsumer<Comando>
    {
        private readonly IEventReceiver _eventReceiver;

        public ComandoConsumer(IEventReceiver eventReceiver)
        {
            _eventReceiver = eventReceiver;
        }

        public async Task Consume(ConsumeContext<Comando> context)
        {
            var comando = context.Message;
            await _eventReceiver.ReceberEvento(comando);
        }
    }
}
