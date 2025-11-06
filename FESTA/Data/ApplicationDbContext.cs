using FESTA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FESTA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Admin> Administradores { get; set; }
        public DbSet<DetalleReserva> DetallesReserva { get; set; }

    }
}
