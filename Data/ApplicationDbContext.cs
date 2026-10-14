using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext(options)
{
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<Book> Books { get; set; } = default!;
    public DbSet<Music> Music { get; set; } = default!;
    public DbSet<Toy> Toys { get; set; } = default!;

    public DbSet<Borrower> Borrowers { get; set; } = default!;
    public DbSet<Loan> Loans { get; set; } = default!;
    public DbSet<LibraryBranch> LibraryBranches { get; set; } = default!;
    public DbSet<BranchTransfer> BranchTransfers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Every physical library item must have a unique library code.
        builder.Entity<Item>()
            .HasIndex(item => item.LibraryCode)
            .IsUnique();

        // Keep loan history if somebody tries to delete an item.
        builder.Entity<Loan>()
            .HasOne(loan => loan.Item)
            .WithMany(item => item.Loans)
            .HasForeignKey(loan => loan.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Keep loan history if somebody tries to delete a borrower.
        builder.Entity<Loan>()
            .HasOne(loan => loan.Borrower)
            .WithMany(borrower => borrower.Loans)
            .HasForeignKey(loan => loan.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BranchTransfer>()
    .HasOne(t => t.FromBranch)
    .WithMany()
    .HasForeignKey(t => t.FromBranchId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BranchTransfer>()
            .HasOne(t => t.ToBranch)
            .WithMany()
            .HasForeignKey(t => t.ToBranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}