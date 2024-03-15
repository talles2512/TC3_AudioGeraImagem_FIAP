using AudioGeraImagem.Domain.Entities;
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
        Task<string> CriarImagem(Stream stream);
        Task<ICollection<Comando>> ListarCriacoes(string busca);
        Task ObterImagem(string id);
    }
}
