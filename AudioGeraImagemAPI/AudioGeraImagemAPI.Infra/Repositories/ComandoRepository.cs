using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;

namespace AudioGeraImagemAPI.Infra.Repositories
{
    public class ComandoRepository : EntidadeBaseRepository<Comando>, IComandoRepository
    {
        public ComandoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
