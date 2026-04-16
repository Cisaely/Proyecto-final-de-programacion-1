using Microsoft.AspNetCore.Mvc;
using NexBusiness.Interfaces;
using NexBusiness.Models;

namespace NexBusiness.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) { _auth = auth; }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto dto)
        {
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
            };
            var result = await _auth.Registrar(usuario, dto.Password);
            if (result == null) return BadRequest(new { mensaje = "El email ya está registrado" });
            
            var token = await _auth.Login(dto.Email, dto.Password);
            return Ok(new { token, id = result.Id, nombre = result.Nombre });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _auth.Login(dto.Email, dto.Password);
            if (token != null) return Ok(new { token });
            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> GetPerfil()
        {
            var idStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (idStr == null) return Unauthorized();
            var user = await _auth.ObtenerUsuario(int.Parse(idStr));
            if (user == null) return NotFound();
            return Ok(new { user.Nombre, user.Email });
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
        {
            var idStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (idStr == null) return Unauthorized();
            
            var userUpdate = new Usuario { Nombre = dto.Nombre, Email = dto.Email, PasswordHash = dto.Password ?? "" };
            var result = await _auth.ActualizarUsuario(int.Parse(idStr), userUpdate);
            if (result == null) return NotFound();
            return Ok(new { result.Nombre, result.Email });
        }
    }

    public record RegistroDto(string Nombre, string Email, string Password);
    public record LoginDto(string Email, string Password);
    public record ActualizarPerfilDto(string Nombre, string Email, string? Password);
}
