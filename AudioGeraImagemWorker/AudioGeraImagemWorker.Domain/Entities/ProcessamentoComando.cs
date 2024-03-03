using AudioGeraImagemWorker.Domain.Enums;
using System.Text.Json.Serialization;

namespace AudioGeraImagemWorker.Domain.Entities
{
    public class ProcessamentoComando
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EstadoComando Estado { get; set; }
        public DateTime InstanteCriacao { get; set; }
        public string MensagemErro { get; set; }
    }
}
