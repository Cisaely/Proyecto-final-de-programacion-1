using System.ComponentModel.DataAnnotations;

namespace NexBusiness.Models
{
    // Usuario dueño del negocio
    public class Usuario : BaseEntity
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PasswordHash { get; set; } = string.Empty;

        // Relación: Un usuario puede tener muchos negocios (o uno)
        public ICollection<Negocio> Negocios { get; set; } = new List<Negocio>();
    }
}
