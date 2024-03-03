using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Infra.Repositories
{
    public class ComandoRepository : EntidadeBaseRepository<Comando>, IComandoRepository
    {
        public ComandoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
