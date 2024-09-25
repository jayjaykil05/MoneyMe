using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MoneyMe.Models;

public partial class MoneymeDbContext : DbContext
{
    public MoneymeDbContext()
    {
    }

    public MoneymeDbContext(DbContextOptions<MoneymeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blacklist> Blacklists { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blacklist>(entity =>
        {
            entity.HasKey(e => e.BlacklistId).HasName("Blacklist_pkey");

            entity.ToTable("Blacklist");

            entity.Property(e => e.BlacklistId)
                .HasDefaultValueSql("nextval('\"Blacklist_blacklis_id_seq\"'::regclass)")
                .HasColumnName("blacklist_id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_created");
            entity.Property(e => e.IsDomain).HasColumnName("is_domain");
            entity.Property(e => e.IsMobile).HasColumnName("is_mobile");
            entity.Property(e => e.Value)
                .HasColumnType("character varying")
                .HasColumnName("value");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("Loans_pkey");

            entity.Property(e => e.LoanId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("loan_id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_created");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_updated");
            entity.Property(e => e.EstablishmentFee).HasColumnName("establishment_fee");
            entity.Property(e => e.FinanceAmount).HasColumnName("finance_amount");
            entity.Property(e => e.InterestFee).HasColumnName("interest_fee");
            entity.Property(e => e.MonthlyAmounn).HasColumnName("monthly_amounn");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Status)
                .HasColumnType("character varying")
                .HasColumnName("status");
            entity.Property(e => e.Term).HasColumnName("term");
            entity.Property(e => e.TotalRepayments).HasColumnName("total_repayments");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Loans)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Loans_user_id_fkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pkey");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("nextval('product_product_id_seq'::regclass)")
                .HasColumnName("product_id");
            entity.Property(e => e.ProductName)
                .HasColumnType("character varying")
                .HasColumnName("product_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DateCreated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_created");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.DateUpdated)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_updated");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.Mobile)
                .HasColumnType("character varying")
                .HasColumnName("mobile");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
