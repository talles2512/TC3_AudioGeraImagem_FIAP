namespace AudioGeraImagem.Domain.Messages
{
    public class RetentativaComandoMessage
    {
        public Guid ComandoId { get; set; }
        public byte[] Payload { get; set; }
    }
}
