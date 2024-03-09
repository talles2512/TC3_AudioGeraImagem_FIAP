namespace AudioGeraImagemWorker.Domain.Entities.Request
{
    public class TranscricaoRequest
    {
        public string model { get; set; }
        public byte[] bytes { get; set; }
        public string filename { get; set; }
    }
}