using AudioGeraImagemAPI.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagemAPI.Domain.Entities
{
    public class Comando : EntidadeBase
    {
        public byte[] Payload { get; set; }
        public string UrlAudio { get; set; }
        public string TextoProcessado { get; set; }
        public string UrlImagem { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public DateTime InstanteAtualizacao { get; set; }
        public List<ProcessamentoComando> ProcessamentosComandos { get; set; }
    }
}
