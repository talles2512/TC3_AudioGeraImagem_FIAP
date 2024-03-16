using AudioGeraImagem.Domain.Messages;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AudioGeraImagemAPI.Domain.Services
{
    public class ComandoService : IComandoService
    {
        readonly IComandoRepository _repository;
        readonly IBus _bus;
        string queueName;

        public ComandoService(IComandoRepository repository, IBus bus, IConfiguration configuration)
        {
            _repository = repository;
            _bus = bus;
            queueName = configuration.GetSection("MassTransit")["NomeFila"] ?? string.Empty;
        }

        public async Task GerarImagem(Comando comando, IFormFile arquivo)
        {
            using var stream = arquivo.OpenReadStream();

            var payload = ObterPayload(stream);
            var mensagem = CriarMensagem(comando, payload);

            await _repository.Inserir(comando);

            await PublicarMensagem(mensagem);
        }

        private byte[] ObterPayload(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            return bytes;
        }

        private ComandoMessage CriarMensagem(Comando comando, byte[] payload)
        {
            return new()
            {
                ComandoId = comando.Id,
                Payload = payload
            };
        }

        private async Task PublicarMensagem(ComandoMessage mensagem)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(mensagem);
        }

        public async Task<ICollection<Comando>> ListarCriacoes(string busca)
        {
            return await _repository.ObterTodos();
        }

        public Task ObterImagem(string id)
        {
            throw new NotImplementedException();
        }
    }
}
