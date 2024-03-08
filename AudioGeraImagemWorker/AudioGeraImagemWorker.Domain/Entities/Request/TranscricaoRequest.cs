namespace AudioGeraImagemWorker.Domain.Entities.Request
{
    public class TranscricaoRequest
    {
        public string file { get; set; }
        public string model { get; set; }
        public string response_format { get; set; }
        public byte[] bytes { get; set; }
        public string filename { get; set; }
    }
}