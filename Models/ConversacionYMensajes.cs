using System.ComponentModel.DataAnnotations;

namespace NexBusiness.Models
{
    public class Conversacion : BaseEntity
    {
        // Sesión anónima del usuario que visita la página
        [Required] public string SessionId { get; set; } = string.Empty;
        
        // Relación: Conversación para un negocio específico
        public int NegocioId { get; set; }
        public Negocio? Negocio { get; set; }

        public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }

    public class Mensaje : BaseEntity
    {
        public int ConversacionId { get; set; }
        public Conversacion? Conversacion { get; set; }

        [Required] public string Texto { get; set; } = string.Empty;
        
        // true si lo envió el usuario, false si lo respondió la IA
        public bool EsUsuario { get; set; } 
    }
}
