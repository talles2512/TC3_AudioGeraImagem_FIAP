namespace AudioGeraImagemWorker.Domain.Entities.Response
{
    public class GerarImagemResponse
    {
        public int created { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string url { get; set; }
    }
}