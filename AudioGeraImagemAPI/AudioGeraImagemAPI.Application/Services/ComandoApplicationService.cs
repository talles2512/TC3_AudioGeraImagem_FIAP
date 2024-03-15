using AudioGeraImagem.Domain.Entities;
using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Domain.Entities;
using AudioGeraImagemAPI.Domain.Enums;
using AudioGeraImagemAPI.Domain.Interfaces;

namespace AudioGeraImagemAPI.Application.Services
{
    public class ComandoApplicationService : IComandoApplicationService
    {
        private readonly IComandoService _service;

        public ComandoApplicationService(IComandoService service)
        {
            _service = service;
        }

        public async Task<string> CriarImagem(Stream stream)
        {
            var bytes = ObterBytes(stream);
            var comando = CriarComando(bytes);

            return await _service.CriarImagem(comando);
        }

        public async Task<ICollection<Comando>> ListarCriacoes(string busca)
        {
            return await _service.ListarCriacoes(busca);
        }

        public Task ObterImagem(string id)
        {
            throw new NotImplementedException();
        }

        private byte[] ObterBytes(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();

            return bytes;
        }

        private Comando CriarComando(byte[] bytes)
        {
            var comando = new Comando()
            {
                Id = Guid.NewGuid(),
                Payload = bytes,
                InstanteCriacao = DateTime.Now,
                ProcessamentosComandos = new()
            };

            comando.ProcessamentosComandos.Add(new()
            {
                Estado = EstadoComando.Recebido,
                InstanteCriacao = DateTime.Now
            });

            return comando;
        }
    }
}
