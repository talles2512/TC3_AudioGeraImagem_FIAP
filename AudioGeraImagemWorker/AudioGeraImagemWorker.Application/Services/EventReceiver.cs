using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Application.Interfaces;
using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Services;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Application.Services
{
    public class EventReceiver : IEventReceiver
    {
        private readonly IComandoManager _comandoManager;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public EventReceiver(IComandoManager comandoManager,
                             ILogger<ComandoManager> logger)
        {
            _comandoManager = comandoManager;
            _logger = logger;
        }

        public async Task ReceberEvento(Comando comando)
        {
            try
            {
                await _comandoManager.ProcessarComando(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [ReceberEvento] => Exception.: {ex.Message}");
            }
        }
    }
}