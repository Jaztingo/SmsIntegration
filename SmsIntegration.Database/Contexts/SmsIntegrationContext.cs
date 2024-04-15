using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SmsIntegration.Database.Models;

namespace SmsIntegration.Database.Contexts;

public partial class SmsIntegrationContext : DbContext
{
    public SmsIntegrationContext(DbContextOptions<SmsIntegrationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SmsFlow> SmsFlows { get; set; }

    public virtual DbSet<SmsProvider> SmsProviders { get; set; }

    public virtual DbSet<SmsProviderConfig> SmsProviderConfigs { get; set; }

    public virtual DbSet<Smss> Smsses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SmsFlow>(entity =>
        {
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Data).HasMaxLength(500);

            entity.HasOne(d => d.Provider).WithMany(p => p.SmsFlows)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("FK_SmsFlows_SmsProviders");

            entity.HasOne(d => d.Sms).WithMany(p => p.SmsFlows)
                .HasForeignKey(d => d.SmsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SmsFlows_Smses");
        });

        modelBuilder.Entity<SmsProvider>(entity =>
        {
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImplementatorName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(500);
        });

        modelBuilder.Entity<SmsProviderConfig>(entity =>
        {
            entity.Property(e => e.ConfigKey)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ConfigValue).HasMaxLength(500);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.SmsProvider).WithMany(p => p.SmsProviderConfigs)
                .HasForeignKey(d => d.SmsProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SmsProviderConfigs_SmsProviders");
        });

        modelBuilder.Entity<Smss>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Smses");

            entity.ToTable("Smss");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.MessageText).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
