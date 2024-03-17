using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Application.ViewModels;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace AudioGeraImagemAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComandoController : ControllerBase
    {
        private readonly IComandoApplicationService _applicationService;
        private readonly ILogger<ComandoController> _logger;
        private readonly string ClassName = typeof(ComandoController).Name;

        public ComandoController(IComandoApplicationService applicationService, ILogger<ComandoController> logger)
        {
            _applicationService = applicationService;
            _logger = logger;
        }

        [HttpPost("gerar-imagem")]
        public async Task<IActionResult> GerarImagem([FromForm] GerarImagemViewModel gerarImagem)
        {
            try
            {
                _logger.LogInformation($"[{ClassName}] - [GerarImagem] => Request.: {gerarImagem.Descricao} - {gerarImagem.Arquivo.FileName}");

                var (sucesso, resultado) = await _applicationService.GerarImagem(gerarImagem);

                if(sucesso)
                    return Accepted(string.Empty, resultado);
                else
                    return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ClassName}] - [GerarImagem] => Exception.: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("buscar-criacoes")]
        public async Task<IActionResult> BuscarCriacoes(string busca = "")
        {
            try
            {
                _logger.LogInformation($"[{ClassName}] - [BuscarCriacoes] => Request.: {new { Busca = busca }}");

                var criacoes = await _applicationService.BuscarCriacoes(busca);

                return Ok(criacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{ClassName}] - [ListarCriacoes] => Exception.: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("obter-criacao/{id}")]
        public async Task<IActionResult> ObterCriacao(string id)
        {
            try
            {
                _logger.LogInformation($"[{ClassName}] - [ObterCriacao] => Request.: {new { Id = id }}");

                var (sucesso, resultado) = await _applicationService.ObterCriacao(id);

                if (sucesso)
                    return Ok(resultado);
                else
                    return NotFound("Criação não encontrada.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
