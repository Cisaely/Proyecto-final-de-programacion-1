using Microsoft.EntityFrameworkCore;
using NexBusiness.Models;

namespace NexBusiness.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Negocio> Negocios { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Conversacion> Conversaciones { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Usuario -> Negocios
            modelBuilder.Entity<Negocio>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Negocios)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Negocio -> Servicios
            modelBuilder.Entity<Servicio>()
                .HasOne(s => s.Negocio)
                .WithMany(n => n.Servicios)
                .HasForeignKey(s => s.NegocioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Negocio -> Conversaciones
            modelBuilder.Entity<Conversacion>()
                .HasOne(c => c.Negocio)
                .WithMany()
                .HasForeignKey(c => c.NegocioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversacion -> Mensajes
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.Conversacion)
                .WithMany(c => c.Mensajes)
                .HasForeignKey(m => m.ConversacionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
