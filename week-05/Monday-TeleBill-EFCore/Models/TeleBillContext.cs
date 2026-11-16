
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace MonTeleBillEFCoreDemo.Models
{
    public partial class TeleBillContext : DbContext
    {
        // SQL Logging Configuration
        public static readonly ILoggerFactory MyLoggerFactory =
            LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information);
            });

        public TeleBillContext()
        {
        }

        public TeleBillContext(DbContextOptions<TeleBillContext> options)
            : base(options)
        {
        }

        // Database Tables
        public virtual DbSet<Bills> Bills { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Plans> Plans { get; set; }
        public virtual DbSet<Subscriptions> Subscriptions { get; set; }

        // Database Connection and SQL Logging
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLoggerFactory(MyLoggerFactory)
                    .EnableSensitiveDataLogging(false)
                    .UseSqlServer(
                        "Server=WIN764OMNIA;Database=TeleBill;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        // Fluent API Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bills>(entity =>
            {
                entity.HasKey(e => e.BillId)
                    .HasName("PK__Bills__11F2FC6A305EE676");

                entity.Property(e => e.BillAmount)
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BillDate)
                    .HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bills__CustomerI__2D27B809");
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.HasKey(e => e.CustomerId)
                    .HasName("PK__Customer__A4AE64D811FFAED2");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Plans>(entity =>
            {
                entity.HasKey(e => e.PlanId)
                    .HasName("PK__Plans__755C22B7C5A93590");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PlanName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Subscriptions>(entity =>
            {
                entity.HasKey(e => e.SubscriptionId)
                    .HasName("PK__Subscrip__9A2B249D2441D080");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Subscript__Custo__29572725");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Subscript__PlanI__2A4B4B5E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
