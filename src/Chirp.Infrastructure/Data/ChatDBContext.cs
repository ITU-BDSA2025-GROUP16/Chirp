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
    public DbSet<Follow> Follows { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Cheep>(b =>
        {
            b.HasKey(c => c.CheepId);
            b.HasOne(c => c.Author)
             .WithMany(a => a.Cheeps)
             .HasForeignKey(c => c.AuthorId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Follow>(b =>
        {
            b.HasKey(f => new { f.FollowerId, f.FollowedId });
            
            b.HasIndex(f => f.FollowerId);
            b.HasIndex(f => f.FollowedId);
        });

        
    }
}