using AudioGeraImagemAPI.Application.Intefaces;
using AudioGeraImagemAPI.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AudioGeraImagemAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComandoController : ControllerBase
    {
        private readonly IComandoApplicationService _applicationService;

        public ComandoController(IComandoApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost("gerar-imagem")]
        public async Task<IActionResult> GerarImagem([FromForm] GerarImagemViewModel gerarImagem)
        {
            try
            {
                var (sucesso, resultado) = await _applicationService.GerarImagem(gerarImagem);

                if(sucesso)
                    return Accepted(string.Empty, resultado);
                else
                    return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("listar-criacoes")]
        public async Task<IActionResult> ListarCriacoes(string busca)
        {
            try
            {
                var criacoes = await _applicationService.ListarCriacoes(busca);

                return Ok(criacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("obter-imagem")]
        public async Task<IActionResult> ObterImagem(string id)
        {
            try
            {
                return Ok("");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
