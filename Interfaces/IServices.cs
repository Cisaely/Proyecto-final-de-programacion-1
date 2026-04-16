using NexBusiness.Models;

namespace NexBusiness.Interfaces
{
    public interface IAuthService
    {
        Task<string?> Login(string email, string password);
        Task<Usuario?> Registrar(Usuario usuario, string password);
        Task<Usuario?> ObtenerUsuario(int id);
        Task<Usuario?> ActualizarUsuario(int id, Usuario datos);
    }

    public interface INegocioService
    {
        Task<List<Negocio>> ObtenerTodos(string? categoria = null);
        Task<List<Negocio>> ObtenerPorUsuario(int usuarioId);
        Task<Negocio?> ObtenerPorId(int id);
        Task<Negocio> Crear(Negocio negocio);
        Task<Negocio?> Actualizar(int id, Negocio datos);
        Task<bool> Eliminar(int id);
    }

    public interface IServicioService
    {
        Task<List<Servicio>> ObtenerPorNegocio(int negocioId);
        Task<Servicio> Crear(Servicio servicio);
        Task<Servicio?> Actualizar(int id, Servicio datos);
        Task<bool> Eliminar(int id);
    }
}
