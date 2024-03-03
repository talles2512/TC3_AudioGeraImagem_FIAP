using AudioGeraImagemAPI.Application.Intefaces;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("cria-imagem")]
        public async Task<IActionResult> CriaImagem(IFormFile formFile)
        {
            try
            {
                using var stream = formFile.OpenReadStream();

                var id = await _applicationService.CriarImagem(stream);

                return Accepted(id);
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
