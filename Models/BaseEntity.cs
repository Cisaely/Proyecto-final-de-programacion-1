namespace NexBusiness.Models
{
    // Clase base abstracta - Principio de OOP: Herencia y Abstracción
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
