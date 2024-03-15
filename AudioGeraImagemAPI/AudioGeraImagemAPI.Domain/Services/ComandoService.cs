using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Interfaces;
using AudioGeraImagemAPI.Domain.Interfaces.Repositories;
using MassTransit;
using Microsoft.Extensions.Configuration;

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

        public async Task<string> CriarImagem(Comando comando)
        {
            await _repository.Inserir(comando);

            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(comando);

            return comando.Id.ToString();
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
