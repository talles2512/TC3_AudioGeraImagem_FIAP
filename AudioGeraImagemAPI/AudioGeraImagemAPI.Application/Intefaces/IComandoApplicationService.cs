﻿using AudioGeraImagemAPI.Application.ViewModels;
using AudioGeraImagemAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Application.Intefaces
{
    public interface IComandoApplicationService
    {
        Task<Tuple<bool, string>> GerarImagem(GerarImagemViewModel gerarImagem);
        Task<ICollection<Comando>> ListarCriacoes(string busca);
        Task ObterImagem(string id);
    }
}
