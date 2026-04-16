using System.ComponentModel.DataAnnotations;

namespace NexBusiness.Models
{
    public class Servicio : BaseEntity
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Precio { get; set; } = string.Empty;

        // Relación: Servicio pertenece a un Negocio
        public int NegocioId { get; set; }
        public Negocio? Negocio { get; set; }
    }
}
