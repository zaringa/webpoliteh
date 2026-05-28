using Lab11.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(author => author.Books)
            .WithOne(book => book.Author)
            .HasForeignKey(book => book.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasMany(category => category.Books)
            .WithOne(book => book.Category)
            .HasForeignKey(book => book.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasIndex(category => category.Name)
            .IsUnique();
    }
}
