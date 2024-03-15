using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemAPI.Infra.Configurations
{
    public class ComandoConfiguration : IEntityTypeConfiguration<Comando>
    {
        public void Configure(EntityTypeBuilder<Comando> builder)
        {
            builder.ToTable("Comandos");
            builder.HasKey(x => x.Id);
            builder.Ignore(x => x.Payload);
            builder.Property(x => x.InstanteCriacao)
                .HasColumnType("DATETIME2");
            builder.Property(x => x.InstanteAtualizacao)
                .HasColumnType("DATETIME2");
            builder.Property(x => x.UrlAudio)
                .HasColumnType("VARCHAR(MAX)");
            builder.Property(x => x.TextoProcessado)
                .HasColumnType("VARCHAR(MAX)");
            builder.Property(x => x.UrlImagem)
                .HasColumnType("VARCHAR(MAX)");

            builder.OwnsMany(x => x.ProcessamentosComandos, procesamentoComando =>
            {
                procesamentoComando.ToTable("ProcessamentoComandos");
                procesamentoComando.HasKey(x => x.Id);
                procesamentoComando.Property(x => x.Estado).HasConversion<string>()
                    .HasColumnType("VARCHAR(20)");
                procesamentoComando.Property(x => x.InstanteCriacao)
                    .HasColumnType("DATETIME2");
                procesamentoComando.Property(x => x.MensagemErro)
                    .HasColumnType("VARCHAR(256)");
            });
        }
    }
}
