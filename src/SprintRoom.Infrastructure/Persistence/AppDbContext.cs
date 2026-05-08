using Microsoft.EntityFrameworkCore;

namespace SprintRoom.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        // Los DbSet para las entidades van aquí
        // DbSet<YourEntity> YourEntities { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Esta línea busca automáticamente todas las clases de configuración
            // que estén en el mismo ensamblado (proyecto) que el AppDbContext.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}