using AudioGeraImagemWorker.Domain.Entities;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ErroManager : IErroManager
    {
        private readonly IComandoManager _comandoManager;
        private readonly IComandoRepository _comandoRepository;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public ErroManager(IComandoManager comandoManager,
                           IComandoRepository comandoRepository,
                           ILogger<ComandoManager> logger,
                           string className)
        {
            _comandoManager = comandoManager;
            _comandoRepository = comandoRepository;
            _logger = logger;
            _className = className;
        }

        public async Task TratarErro(Comando comando)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            // Se por acaso um processamento falhar mais de 3 vezes num "estado"
            if(ultimoProcessamento.Tentativa > 3)
            {
                comando.InstanteAtualizacao = DateTime.Now;

                var novoProcessamentoComando = new ProcessamentoComando()
                {
                    Estado = Enums.EstadoComando.Falha,
                    InstanteCriacao = DateTime.Now,
                    Tentativa = ultimoProcessamento.Tentativa, //Toda vez que receber um novo comando zeramos a tentativa.
                    MensagemErro = ultimoProcessamento.MensagemErro
                };

                comando.ProcessamentosComandos.Add(novoProcessamentoComando);
                _logger.LogError($"[{_className}] - [TratarErro] => Erro.: Excesso de tentativas no estado.: {ultimoProcessamento.Estado} | Tentativa.: {ultimoProcessamento.Tentativa} | Finalizando com estado.: Falha");
            }
            else
            {
                //Acredito que esse codigo pegue o Penultimo Processamento kkk
                var penultimoProcessamento = comando.ProcessamentosComandos.ElementAtOrDefault(comando.ProcessamentosComandos.Count() - 2);
                comando.InstanteAtualizacao = DateTime.Now;

                var novoProcessamentoComando = new ProcessamentoComando()
                {
                    Estado = penultimoProcessamento.Estado,
                    InstanteCriacao = DateTime.Now,
                    Tentativa = penultimoProcessamento.Tentativa + 1, //Toda vez que receber um novo comando zeramos a tentativa.
                    MensagemErro = penultimoProcessamento.MensagemErro
                };
                
                comando.ProcessamentosComandos.Add(novoProcessamentoComando);
                _logger.LogError($"[{_className}] - [TratarErro] => Erro.: Reprocessamento do estado.: {ultimoProcessamento.Estado} | Tentativa.: {ultimoProcessamento.Tentativa}");
            }

            await _comandoRepository.Atualizar(comando);
            await Task.Delay(20000); //Aguarda 20 segundos para o reprocessamento
            await _comandoManager.ProcessarComando(comando);
            
        }
    }
}