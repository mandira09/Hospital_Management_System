using Hospital_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace Hospital_Management.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Billing> Billings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🚀 NO DOUBLE BOOKING RULE (DB LEVEL)
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.DateTime })
                .IsUnique();
        }
    }
}
