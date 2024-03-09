using AudioGeraImagemWorker.Domain.Entities.Request;
using AudioGeraImagemWorker.Domain.Entities.Response;
using AudioGeraImagemWorker.Domain.Interfaces.Vendor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AudioGeraImagemWorker.Infra.Utility.Vendor
{
    public class OpenAIVendor : IOpenAIVendor
    {
        private readonly HttpHelp _httpHelp;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BucketManager> _logger;
        private readonly string _className = typeof(BucketManager).Name;

        public OpenAIVendor(HttpHelp httpHelp,
                            IConfiguration configuration,
                            ILogger<BucketManager> logger)
        {
            _httpHelp = httpHelp;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Tuple<GerarImagemResponse, string>> GerarImagem(GerarImagemRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var url = _configuration.GetSection("OpenAI")["GenerateImageEndpoint"];
                var secret = _configuration.GetSection("OpenAI")["APIKey"];
                Dictionary<string, string> Dicheaders = new Dictionary<string, string>();

                //Add Headers
                Dicheaders = AddHeaders(Dicheaders, "Authorization", $"Bearer {secret}");

                //Integração externa API OpenAI - Utilitario
                var result = await _httpHelp.Send(url, json, VerboHttp.Post, Dicheaders, null);

                if (result.Code.ToString() == "Success")
                {
                    var response = JsonSerializer.Deserialize<GerarImagemResponse>(result.Received);
                    return new Tuple<GerarImagemResponse, string>(response, "");
                }
                else
                {
                    var response = JsonSerializer.Deserialize<ErrorResponse>(result.Received);
                    return new Tuple<GerarImagemResponse, string>(null, response.error.message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [GerarImagem] => Exception.: {ex.Message}");
                return new Tuple<GerarImagemResponse, string>(null, "Exception");
            }
        }

        public async Task<Tuple<TranscricaoResponse, string>> Transcricao(TranscricaoRequest request)
        {
            try
            {
                var url = _configuration.GetSection("OpenAI")["TranscriptionEndpoint"];
                var secret = _configuration.GetSection("OpenAI")["APIKey"];

                var multiPartContent = new MultipartFormDataContent
            {
               // Adicionando parâmetros de texto:
               { new StringContent(request.model), "model" }
               //{ new StringContent(request.response_format), "response_format" }
            };

                var arquivoContent = new ByteArrayContent(request.bytes);
                multiPartContent.Add(arquivoContent, "file", $"{request.filename}");

                Dictionary<string, string> Dicheaders = new Dictionary<string, string>();
                //Add Headers
                Dicheaders = AddHeaders(Dicheaders, "Authorization", $"Bearer {secret}");

                //Integração externa API OpenAI - Utilitario
                var result = await _httpHelp.Send(url, "", VerboHttp.Post, Dicheaders, multiPartContent);

                if (result.Code.ToString() == "Success")
                {
                    var response = JsonSerializer.Deserialize<TranscricaoResponse>(result.Received);
                    return new Tuple<TranscricaoResponse, string>(response, "");
                }
                else
                {
                    var response = JsonSerializer.Deserialize<ErrorResponse>(result.Received);
                    return new Tuple<TranscricaoResponse, string>(null, response.error.message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{_className}] - [Transcricao] => Exception.: {ex.Message}");
                return new Tuple<TranscricaoResponse, string>(null, "Exception");
            }
        }

        private Dictionary<string, string> AddHeaders(Dictionary<string, string> headers,
                                                     string key,
                                                     string value)
        {
            headers.Add(key, value);
            return headers;
        }
    }
}