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
        private readonly IComandoRepository _repository;
        private readonly IBus _bus;
        private string nomeFila;

        public ComandoService(IComandoRepository repository, IBus bus, IConfiguration configuration)
        {
            _repository = repository;
            _bus = bus;
            nomeFila = configuration.GetRequiredSection("MassTransit")["Fila"] ?? string.Empty;
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
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{nomeFila}"));
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
