using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NexBusiness.Data;
using NexBusiness.Interfaces;
using NexBusiness.Models;

namespace NexBusiness.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<string?> Login(string email, string password)
        {
            var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            return GenerarToken(user.Id.ToString(), user.Email, user.Nombre);
        }

        public async Task<Usuario?> Registrar(Usuario usuario, string password)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Email == usuario.Email)) return null;
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> ObtenerUsuario(int id)
        {
            return await _db.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> ActualizarUsuario(int id, Usuario datos)
        {
            var user = await _db.Usuarios.FindAsync(id);
            if (user == null) return null;
            
            user.Nombre = datos.Nombre;
            user.Email = datos.Email;
            if(!string.IsNullOrEmpty(datos.PasswordHash)) {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(datos.PasswordHash);
            }
            await _db.SaveChangesAsync();
            return user;
        }

        private string GenerarToken(string id, string email, string nombre)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["Jwt:Key"] ?? "NexBusinessSecretKey2026SuperSegura!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Email, email),
                new Claim("Nombre", nombre)
            };
            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(7),
                claims: claims,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
