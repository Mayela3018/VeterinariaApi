using Microsoft.EntityFrameworkCore;
using VeterinariaApi.Models;

namespace VeterinariaApi.Data;

public partial class VeterinariaContext : DbContext
{
    public VeterinariaContext(DbContextOptions<VeterinariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dueno> Duenos { get; set; }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dueno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Duenos");
            entity.Property(e => e.Dni).IsFixedLength();
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Mascotas");

            entity.HasOne(d => d.Dueno)
                .WithMany(p => p.Mascotas)
                .HasForeignKey(d => d.DuenoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Mascotas_Duenos");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}