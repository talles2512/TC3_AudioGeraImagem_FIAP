using AudioGeraImagemWorker.Domain.Entities.Request;
using AudioGeraImagemWorker.Domain.Entities.Response;

namespace AudioGeraImagemWorker.Domain.Interfaces.Vendor
{
    public interface IOpenAIVendor
    {
        Task<Tuple<TranscricaoResponse, string>> Transcricao(TranscricaoRequest request);

        Task<Tuple<GerarImagemResponse, string>> GerarImagem(GerarImagemRequest request);
    }
}