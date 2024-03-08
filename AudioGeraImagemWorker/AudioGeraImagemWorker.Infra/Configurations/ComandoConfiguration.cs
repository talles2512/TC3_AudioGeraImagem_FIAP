using AudioGeraImagemWorker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AudioGeraImagemWorker.Infra.Configurations
{
    public class ComandoConfiguration : IEntityTypeConfiguration<Comando>
    {
        public void Configure(EntityTypeBuilder<Comando> builder)
        {
            builder.ToTable("Comandos");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.InstanteCriacao)
                .HasColumnType("DATETIME");
            builder.Property(x => x.InstanteAtualizacao)
                .HasColumnType("DATETIME");

            builder.OwnsMany(x => x.ProcessamentosComandos, procesamentoComando =>
            {
                procesamentoComando.ToTable("ProcessamentoComandos");
                procesamentoComando.Property(x => x.Estado).HasConversion<string>()
                    .HasColumnType("VARCHAR(20)");
                procesamentoComando.Property(x => x.InstanteCriacao)
                    .HasColumnType("DATETIME");
                procesamentoComando.Property(x => x.MensagemErro)
                    .HasColumnType("VARCHAR(256)");
            });
        }
    }
}