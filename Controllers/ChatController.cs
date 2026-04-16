using Microsoft.AspNetCore.Mvc;
using NexBusiness.Interfaces;
using NexBusiness.Models;
using NexBusiness.Services;

namespace NexBusiness.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly INegocioService _negocioService;

        public ChatController(IChatService chatService, INegocioService negocioService)
        {
            _chatService = chatService;
            _negocioService = negocioService;
        }

        [HttpPost("negocio/{id}")]
        public async Task<IActionResult> EnviarMensaje(int id, [FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Mensaje) || string.IsNullOrWhiteSpace(request.SessionId))
                return BadRequest(new { error = "El mensaje o la sesión no pueden estar vacíos." });

            var negocio = await _negocioService.ObtenerPorId(id);
            if (negocio == null)
                return NotFound(new { error = "Negocio no encontrado." });

            var respuestaIA = await _chatService.ProcesarMensajeIA(request.SessionId, negocio, request.Mensaje);
            
            return Ok(new { respuesta = respuestaIA });
        }
    }

    public record ChatRequest(string Mensaje, string SessionId);
}
