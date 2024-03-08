namespace AudioGeraImagemWorker.Domain.Entities.Response
{
    public class TranscricaoResponse
    {
        public string task { get; set; }
        public string language { get; set; }
        public float duration { get; set; }
        public string text { get; set; }
        public Segment[] segments { get; set; }
    }

    public class Segment
    {
        public int id { get; set; }
        public int seek { get; set; }
        public float start { get; set; }
        public float end { get; set; }
        public string text { get; set; }
        public int[] tokens { get; set; }
        public float temperature { get; set; }
        public float avg_logprob { get; set; }
        public float compression_ratio { get; set; }
        public float no_speech_prob { get; set; }
    }

    public class AllTranscription
    {
        public string texts { get; set; }
    }
}