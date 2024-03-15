﻿using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemWorker.Domain.Enums;
using AudioGeraImagemWorker.Domain.Factories;
using AudioGeraImagemWorker.Domain.Interfaces;
using AudioGeraImagemWorker.Domain.Interfaces.Repositories;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using AudioGeraImagemWorker.Domain.Utility;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemWorker.Domain.Services
{
    public class ComandoManager : IComandoManager
    {
        private readonly HttpHelper _httpHelper;
        private readonly IComandoRepository _comandoRepository;
        private readonly IErroManager _erroManager;
        private readonly IBucketManager _bucketManager;
        private readonly IOpenAIVendor _openAIVendor;
        private readonly ILogger<ComandoManager> _logger;
        private readonly string _className = typeof(ComandoManager).Name;

        public ComandoManager(
            HttpHelper httpHelper,
            IComandoRepository comandoRepository,
            IErroManager erroManager,
            IBucketManager bucketManager,
            IOpenAIVendor openAIVendor,
            ILogger<ComandoManager> logger)
        {
            _httpHelper = httpHelper;
            _comandoRepository = comandoRepository;
            _erroManager = erroManager;
            _bucketManager = bucketManager;
            _openAIVendor = openAIVendor;
            _logger = logger;
        }

        public async Task ProcessarComando(Comando comando)
        {
            await AtualizarProcessamentoComando(comando);
            await ExecutarComando(comando);
        }

        public async Task ExecutarComando(Comando comando)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            try
            {
                switch (ultimoProcessamento.Estado)
                {
                    case EstadoComando.SalvandoAudio:
                        await SalvarAudio(comando);
                        break;

                    case EstadoComando.GerandoTexto:
                        await GerarTexto(comando);
                        break;

                    case EstadoComando.GerandoImagem:
                        await GerarImagem(comando);
                        break;

                    case EstadoComando.SalvadoImagem:
                        await SalvarImagem(comando);
                        break;

                    case EstadoComando.Finalizado:
                        await Finalizar(comando);
                        return;
                }

                await AtualizarProcessamentoComando(comando);
                await ExecutarComando(comando);
            }
            catch (Exception ex)
            {
                ultimoProcessamento.MensagemErro = ex.Message;
                await _erroManager.TratarErro(comando, ultimoProcessamento.Estado);
            }
        }

        private async Task AtualizarProcessamentoComando(Comando comando)
        {
            var ultimoProcessamento = comando.ProcessamentosComandos.LastOrDefault();

            EstadoComando novoEstadoComando = default;

            switch (ultimoProcessamento.Estado)
            {
                case EstadoComando.Recebido:
                    novoEstadoComando = EstadoComando.SalvandoAudio;
                    break;

                case EstadoComando.SalvandoAudio:
                    novoEstadoComando = EstadoComando.GerandoTexto;
                    break;

                case EstadoComando.GerandoTexto:
                    novoEstadoComando = EstadoComando.GerandoImagem;
                    break;

                case EstadoComando.GerandoImagem:
                    novoEstadoComando = EstadoComando.SalvadoImagem;
                    break;

                case EstadoComando.SalvadoImagem:
                    novoEstadoComando = EstadoComando.Finalizado;
                    break;
            }

            comando.InstanteAtualizacao = DateTime.Now;

            var novoProcessamentoComando = ProcessamentoComandoFactory.Novo(novoEstadoComando);

            comando.ProcessamentosComandos.Add(novoProcessamentoComando);

            await _comandoRepository.Atualizar(comando);
        }

        #region [ Tratamentos dos Estados dos Comandos ]

        // 1. Estado Recebido >> Salvando Audio
        private async Task SalvarAudio(Comando comando)
        {
            try
            {
                var fileName = string.Concat(comando.Id.ToString(), ".mp3");
                comando.UrlAudio = await _bucketManager.ArmazenarObjeto(comando.Payload, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarAudio] => Exception.: {ex.Message}");
                throw;
            }   
        }

        // 2. Salvando Audio >> Gerando Texto
        private async Task GerarTexto(Comando comando)
        {
            try
            {
                comando.Transcricao = await _openAIVendor.GerarTranscricao(comando.Payload);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarTexto] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 3. Salvando Texto >> Gerando Imagem
        private async Task GerarImagem(Comando comando)
        {
            try
            {
                comando.UrlImagem = await _openAIVendor.GerarImagem(comando.Transcricao);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarImagem] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 4. Gerando Imagem >> Salvado Imagem
        private async Task SalvarImagem(Comando comando)
        {
            try
            {
                var bytes = await _httpHelper.GetBytes(comando.UrlImagem);
                var fileName = string.Concat(comando.Id.ToString(), ".jpeg");
                var urlImagem = await _bucketManager.ArmazenarObjeto(bytes, fileName);
                comando.UrlImagem = urlImagem;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [SalvarImagem] => Exception.: {ex.Message}");
                throw;
            }
        }

        // 5. Salvando Imagem >> Finalizado
        private async Task Finalizar(Comando comando)
        {
            try
            {
                await _comandoRepository.Atualizar(comando);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Finalizar] => Exception.: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}