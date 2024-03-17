using AudioGeraImagemAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AudioGeraImagemAPI.Domain.Interfaces
{
    public interface IComandoService
    {
        Task GerarImagem(Comando comando, IFormFile arquivo);
        Task<Comando> ObterComando(string id);
        Task<ICollection<Comando>> ObterComandosProcessamentos();
        Task<ICollection<Comando>> Buscar(string busca);
    }
}
