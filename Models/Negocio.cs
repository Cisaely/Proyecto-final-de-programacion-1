using System.ComponentModel.DataAnnotations;

namespace NexBusiness.Models
{
    // Negocio (Directorio)
    public class Negocio : BaseEntity
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string? Horario { get; set; }
        public string? Direccion { get; set; }
        
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? SitioWeb { get; set; }
        public string? LogoUrl { get; set; }

        // Relación: Negocio pertenece a un Usuario (dueño)
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Relación: Un negocio ofrece varios servicios
        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    }
}
