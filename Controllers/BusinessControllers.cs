using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NexBusiness.Data;
using NexBusiness.Interfaces;
using NexBusiness.Models;

namespace NexBusiness.Controllers
{
    [ApiController]
    [Route("api/negocios")]
    public class NegociosController : ControllerBase
    {
        private readonly INegocioService _negocios;
        public NegociosController(INegocioService negocios)
        {
            _negocios = negocios;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos([FromQuery] string? categoria) =>
            Ok(await _negocios.ObtenerTodos(categoria));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUno(int id)
        {
            var negocio = await _negocios.ObtenerPorId(id);
            return negocio == null ? NotFound() : Ok(negocio);
        }

        [HttpGet("mis-negocios")]
        [Authorize]
        public async Task<IActionResult> MisNegocios()
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await _negocios.ObtenerPorUsuario(int.Parse(idStr!)));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] Negocio negocio)
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            negocio.UsuarioId = int.Parse(idStr!);
            var result = await _negocios.Crear(negocio);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Negocio datos)
        {
            // Opcional: verificar que el usuario sea el dueño
            var result = await _negocios.Actualizar(id, datos);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _negocios.Eliminar(id);
            return ok ? Ok(new { mensaje = "Eliminado" }) : NotFound();
        }

        [HttpGet("estadisticas")]
        [Authorize]
        public async Task<IActionResult> GetEstadisticas([FromServices] AppDbContext db)
        {
            var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (idStr == null) return Unauthorized();
            int uId = int.Parse(idStr);

            int totalNegocios = await db.Negocios.CountAsync(n => n.UsuarioId == uId);
            
            // Contamos conversaciones que pertenezcan a los negocios de este usuario
            int totalChats = await db.Conversaciones.CountAsync(c => c.Negocio != null && c.Negocio.UsuarioId == uId);
            
            // Contamos los mensajes que la IA ha generado
            int mensajesIA = await db.Mensajes.CountAsync(m => m.Conversacion != null && m.Conversacion.Negocio != null && m.Conversacion.Negocio.UsuarioId == uId && !m.EsUsuario);

            return Ok(new {
                negocios = totalNegocios,
                conversaciones = totalChats,
                mensajesAsistente = mensajesIA
            });
        }
    }

    [ApiController]
    [Route("api/servicios")]
    public class ServiciosController : ControllerBase
    {
        private readonly IServicioService _servicios;
        public ServiciosController(IServicioService servicios)
        {
            _servicios = servicios;
        }

        [HttpGet("negocio/{negocioId}")]
        public async Task<IActionResult> GetPorNegocio(int negocioId) =>
            Ok(await _servicios.ObtenerPorNegocio(negocioId));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] Servicio servicio)
        {
            var result = await _servicios.Crear(servicio);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Servicio datos)
        {
            var result = await _servicios.Actualizar(id, datos);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Eliminar(int id)
        {
            var ok = await _servicios.Eliminar(id);
            return ok ? Ok(new { mensaje = "Eliminado" }) : NotFound();
        }
    }
}
