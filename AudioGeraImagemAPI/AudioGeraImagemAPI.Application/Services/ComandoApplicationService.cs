﻿using AudioGeraImagemAPI.Application.Factory;
using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Application.ViewModels;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces;

namespace AudioGeraImagemAPI.Application.Services
{
    public class ComandoApplicationService : IComandoApplicationService
    {
        private readonly IComandoService _service;

        public ComandoApplicationService(IComandoService service)
        {
            _service = service;
        }

        public async Task<ICollection<ListarCriacaoViewModel>> BuscarCriacoes(string busca)
        {
            List<Comando> listaComandos = new List<Comando>();

            if (string.IsNullOrEmpty(busca))
            {
                var retorno = await _service.ObterComandosProcessamentos();
                listaComandos.AddRange(retorno.ToList());
            }
            else
            {
                var retorno = await _service.Buscar(busca);
                listaComandos.AddRange(retorno.ToList());
            }

            var listaCriacoesViewModels = listaComandos.Select(ViewModelFactory.CriarListarCriacoesViewModel).ToList();

            return listaCriacoesViewModels;
        }

        public async Task<Tuple<bool, ObterCriacaoViewModel>> ObterCriacao(string id)
        {
            var comando = await _service.ObterComando(id);

            if(comando == null)
                return Tuple.Create(false, new ObterCriacaoViewModel());

            return Tuple.Create(true, ViewModelFactory.CriarObterCriacaoViewModel(comando));
        }

        public async Task<Tuple<bool, string>> GerarImagem(GerarImagemViewModel gerarImagem)
        {
            if (!RequisicaoValida(gerarImagem))
                return Tuple.Create(false, "Escreva uma descrição com até 256 caracteres e o arquivo deve ser .mp3");

            var comando = CriarComando(gerarImagem);

            await _service.GerarImagem(comando, gerarImagem.Arquivo);

            return Tuple.Create(true, comando.Id.ToString());
        }

        private bool RequisicaoValida(GerarImagemViewModel gerarImagem)
        {
            if (gerarImagem.Descricao.Length > 256)
                return false;

            if (!gerarImagem.Arquivo.ContentType.Contains("audio/mpeg"))
                return false;

            return true;
        }

        private Comando CriarComando(GerarImagemViewModel gerarImagem)
        {
            var comando = new Comando()
            {
                Id = Guid.NewGuid(),
                Descricao = gerarImagem.Descricao,
                InstanteCriacao = DateTime.Now,
                ProcessamentosComandos = new()
            };

            comando.ProcessamentosComandos.Add(new()
            {
                Estado = EstadoComando.Recebido,
                InstanteCriacao = DateTime.Now
            });

            return comando;
        }
    }
}
