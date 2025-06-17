using Microsoft.EntityFrameworkCore;
using TransactionService.Models.Entity;

namespace TransactionService.Db;

public class TransactionDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<TransactionRecord> TransactionRecords { get; set; }

    public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Balance)
                .WithOne(b => b.Client)
                .HasForeignKey<Balance>(b => b.ClientId);

            entity.HasMany(e => e.Transactions)
                .WithOne(t => t.Client)
                .HasForeignKey(t => t.ClientId);
        });

        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(b => b.ClientId);
            entity.Property(b => b.Amount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<TransactionRecord>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(t => t.Type).IsRequired();
            entity.Property(t => t.Status).IsRequired();
            entity.Property(t => t.CreatedAt).IsRequired();

            entity.HasIndex(t => t.Id).IsUnique();
        });
    }
}