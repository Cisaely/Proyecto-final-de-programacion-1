using Microsoft.EntityFrameworkCore;
using NexBusiness.Data;
using NexBusiness.Interfaces;
using NexBusiness.Models;

namespace NexBusiness.Services
{
    public class NegocioService : INegocioService
    {
        private readonly AppDbContext _db;
        public NegocioService(AppDbContext db) { _db = db; }

        public async Task<List<Negocio>> ObtenerTodos(string? categoria = null)
        {
            var q = _db.Negocios.Include(n => n.Servicios).AsQueryable();
            if (!string.IsNullOrEmpty(categoria)) q = q.Where(n => n.Categoria == categoria);
            return await q.ToListAsync();
        }

        public async Task<List<Negocio>> ObtenerPorUsuario(int usuarioId) =>
            await _db.Negocios.Include(n => n.Servicios).Where(n => n.UsuarioId == usuarioId).ToListAsync();

        public async Task<Negocio?> ObtenerPorId(int id) =>
            await _db.Negocios.Include(n => n.Servicios).FirstOrDefaultAsync(n => n.Id == id);

        public async Task<Negocio> Crear(Negocio negocio)
        {
            _db.Negocios.Add(negocio);
            await _db.SaveChangesAsync();
            return negocio;
        }

        public async Task<Negocio?> Actualizar(int id, Negocio datos)
        {
            var negocio = await _db.Negocios.FindAsync(id);
            if (negocio == null) return null;
            negocio.Nombre = datos.Nombre;
            negocio.Descripcion = datos.Descripcion;
            negocio.Categoria = datos.Categoria;
            negocio.Horario = datos.Horario;
            negocio.Direccion = datos.Direccion;
            negocio.Telefono = datos.Telefono;
            negocio.Email = datos.Email;
            negocio.SitioWeb = datos.SitioWeb;
            if(datos.Latitud.HasValue) negocio.Latitud = datos.Latitud;
            if(datos.Longitud.HasValue) negocio.Longitud = datos.Longitud;
            
            await _db.SaveChangesAsync();
            return negocio;
        }

        public async Task<bool> Eliminar(int id)
        {
            var negocio = await _db.Negocios.FindAsync(id);
            if (negocio == null) return false;
            _db.Negocios.Remove(negocio);
            await _db.SaveChangesAsync();
            return true;
        }
    }

    public class ServicioService : IServicioService
    {
        private readonly AppDbContext _db;
        public ServicioService(AppDbContext db) { _db = db; }

        public async Task<List<Servicio>> ObtenerPorNegocio(int negocioId) =>
            await _db.Servicios.Where(s => s.NegocioId == negocioId).ToListAsync();

        public async Task<Servicio> Crear(Servicio servicio)
        {
            _db.Servicios.Add(servicio);
            await _db.SaveChangesAsync();
            return servicio;
        }

        public async Task<Servicio?> Actualizar(int id, Servicio datos)
        {
            var servicio = await _db.Servicios.FindAsync(id);
            if (servicio == null) return null;
            servicio.Nombre = datos.Nombre;
            servicio.Descripcion = datos.Descripcion;
            servicio.Precio = datos.Precio;
            await _db.SaveChangesAsync();
            return servicio;
        }

        public async Task<bool> Eliminar(int id)
        {
            var servicio = await _db.Servicios.FindAsync(id);
            if (servicio == null) return false;
            _db.Servicios.Remove(servicio);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
