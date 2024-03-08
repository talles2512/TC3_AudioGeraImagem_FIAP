namespace AudioGeraImagemWorker.Domain.Interfaces.Vendor
{
    public interface IBucketManager
    {
        Task<string> ArmazenarAudio(byte[] bytes);
    }
}