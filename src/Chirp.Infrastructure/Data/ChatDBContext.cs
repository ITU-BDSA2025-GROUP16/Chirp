using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Domain;

namespace Chirp.Infrastructure.Data;

public class ChatDBContext : IdentityDbContext<Author, IdentityRole<int>, int>
{
    public ChatDBContext(DbContextOptions<ChatDBContext> options)
        : base(options)
    {
    }

    public DbSet<Cheep> Cheeps { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Map AuthorId as PK for Author
        builder.Entity<Author>(b =>
        {
            b.HasKey(a => a.AuthorId);
            b.Property(a => a.Id).HasColumnName("AuthorId"); // Identity uses Id internally
            b.HasMany(a => a.Cheeps)
             .WithOne(c => c.Author)
             .HasForeignKey(c => c.AuthorId);
        });

        // Configure Cheep
        builder.Entity<Cheep>(b =>
        {
            b.HasKey(c => c.CheepId);
        });
    }
}
