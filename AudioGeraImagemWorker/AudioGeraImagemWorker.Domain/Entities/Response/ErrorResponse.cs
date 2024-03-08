namespace AudioGeraImagemWorker.Domain.Entities.Response
{
    public class ErrorResponse
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public string type { get; set; }
        public object param { get; set; }
        public string code { get; set; }
    }
}