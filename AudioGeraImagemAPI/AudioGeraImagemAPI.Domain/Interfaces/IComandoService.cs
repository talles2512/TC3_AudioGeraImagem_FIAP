using AudioGeraImagemAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AudioGeraImagemAPI.Domain.Interfaces
{
    public interface IComandoService
    {
        Task GerarImagem(Comando comando, IFormFile arquivo);
        Task<ICollection<Comando>> ListarCriacoes(string busca);
        Task ObterImagem(string id);
    }
}
