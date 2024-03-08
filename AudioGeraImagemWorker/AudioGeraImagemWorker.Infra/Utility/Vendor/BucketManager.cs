using AudioGeraImagemWorker.Domain.Interfaces.Vendor;

namespace AudioGeraImagemWorker.Infra.Utility.Vendor
{
    public class BucketManager : IBucketManager
    {
        public async Task<string> ArmazenarAudio(byte[] bytes)
        {
            await Task.Delay(1000);
            throw new NotImplementedException();
        }
    }
}