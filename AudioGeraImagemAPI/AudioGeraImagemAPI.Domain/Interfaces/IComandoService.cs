using AudioGeraImagemAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Domain.Interfaces
{
    public interface IComandoService
    {
        Task<string> CriarImagem(Comando comando);
        Task<ICollection<Comando>> ListarCriacoes(string busca);
        Task ObterImagem(string id);
    }
}
