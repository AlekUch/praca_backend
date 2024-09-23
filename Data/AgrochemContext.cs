using System;
using System.Collections.Generic;
using AGROCHEM.Models;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Data;

public partial class AgrochemContext : DbContext
{
    public AgrochemContext()
    {
    }

    public AgrochemContext(DbContextOptions<AgrochemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChemicalAgent> ChemicalAgents { get; set; }

    public virtual DbSet<ChemicalTreatment> ChemicalTreatments { get; set; }

    public virtual DbSet<ChemicalUse> ChemicalUses { get; set; }

    public virtual DbSet<Cultivation> Cultivations { get; set; }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Plant> Plants { get; set; }

    public virtual DbSet<PlantDisease> PlantDiseases { get; set; }

    public virtual DbSet<Plot> Plots { get; set; }

    public virtual DbSet<PlotAddress> PlotAddresses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=AGROCHEMConn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChemicalAgent>(entity =>
        {
            entity.HasKey(e => e.ChemAgentId).HasName("PK__Chemical__C41C86D6938BE878");

            entity.ToTable("ChemicalAgent", "agro_chem");

            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<ChemicalTreatment>(entity =>
        {
            entity.HasKey(e => e.ChemTreatId).HasName("PK__Chemical__2CFBF0BF3FA16EB3");

            entity.ToTable("ChemicalTreatment", "agro_chem");

            entity.Property(e => e.Area).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Dose).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Reason).HasMaxLength(255);

            entity.HasOne(d => d.ChemAgent).WithMany(p => p.ChemicalTreatments)
                .HasForeignKey(d => d.ChemAgentId)
                .HasConstraintName("FK__ChemicalT__ChemA__3C34F16F");

            entity.HasOne(d => d.Plant).WithMany(p => p.ChemicalTreatments)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("FK__ChemicalT__Plant__3B40CD36");

            entity.HasOne(d => d.Plot).WithMany(p => p.ChemicalTreatments)
                .HasForeignKey(d => d.PlotId)
                .HasConstraintName("FK__ChemicalT__PlotI__3A4CA8FD");
        });

        modelBuilder.Entity<ChemicalUse>(entity =>
        {
            entity.HasKey(e => e.ChemUseId).HasName("PK__Chemical__8DFE527D2FA71BBD");

            entity.ToTable("ChemicalUse", "agro_chem");

            entity.Property(e => e.MaxDose).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.MinDose).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.ChemAgent).WithMany(p => p.ChemicalUses)
                .HasForeignKey(d => d.ChemAgentId)
                .HasConstraintName("FK__ChemicalU__ChemA__37703C52");

            entity.HasOne(d => d.Plant).WithMany(p => p.ChemicalUses)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("FK__ChemicalU__Plant__367C1819");
        });

        modelBuilder.Entity<Cultivation>(entity =>
        {
            entity.HasKey(e => e.CultivationId).HasName("PK__Cultivat__6B84BC3861D273F2");

            entity.ToTable("Cultivation", "agro_chem");

            entity.Property(e => e.Area).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.HarvestDate).HasColumnType("datetime");
            entity.Property(e => e.SowingDate).HasColumnType("datetime");

            entity.HasOne(d => d.Plant).WithMany(p => p.Cultivations)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("FK__Cultivati__Plant__339FAB6E");

            entity.HasOne(d => d.Plot).WithMany(p => p.Cultivations)
                .HasForeignKey(d => d.PlotId)
                .HasConstraintName("FK__Cultivati__PlotI__32AB8735");
        });

        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.DiseaseId).HasName("PK__Disease__69B53389FD793224");

            entity.ToTable("Disease", "agro_chem");

            entity.HasOne(d => d.Photo).WithMany(p => p.Diseases)
                .HasForeignKey(d => d.PhotoId)
                .HasConstraintName("FK__Disease__PhotoId__282DF8C2");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__Photo__21B7B5E28BA83CA6");

            entity.ToTable("Photo", "agro_chem");

            entity.Property(e => e.Extension).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.PlantId).HasName("PK__Plant__98FE395C7612A201");

            entity.ToTable("Plant", "agro_chem");

            entity.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<PlantDisease>(entity =>
        {
            entity.HasKey(e => e.PlDiseId).HasName("PK__PlantDis__80ADE8145F484A49");

            entity.ToTable("PlantDisease", "agro_chem");

            entity.HasOne(d => d.Disease).WithMany(p => p.PlantDiseases)
                .HasForeignKey(d => d.DiseaseId)
                .HasConstraintName("FK__PlantDise__Disea__2EDAF651");

            entity.HasOne(d => d.Plant).WithMany(p => p.PlantDiseases)
                .HasForeignKey(d => d.PlantId)
                .HasConstraintName("FK__PlantDise__Plant__2FCF1A8A");
        });

        modelBuilder.Entity<Plot>(entity =>
        {
            entity.HasKey(e => e.PlotId).HasName("PK__Plot__78FA3AA32768209A");

            entity.ToTable("Plot", "agro_chem");

            entity.Property(e => e.Area).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.PlotNumber).HasMaxLength(50);

            entity.HasOne(d => d.Address).WithMany(p => p.Plots)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__Plot__AddressId__1DB06A4F");

            entity.HasOne(d => d.Owner).WithMany(p => p.Plots)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK__Plot__OwnerId__1CBC4616");
        });

        modelBuilder.Entity<PlotAddress>(entity =>
        {
            entity.HasKey(e => e.PlotAddressId).HasName("PK__PlotAddr__86A9D57858299DE1");

            entity.ToTable("PlotAddress", "agro_chem");

            entity.Property(e => e.District).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Voivodeship).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A3D14BC7A");

            entity.ToTable("Role", "agro_chem");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CEA0B52A2");

            entity.ToTable("User", "agro_chem");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(256);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleId__17F790F9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
