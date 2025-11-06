using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Domain;

namespace Chirp.Infrastructure.Data;

public class ChatDBContext : IdentityDbContext<IdentityUser>
{
    public ChatDBContext(DbContextOptions<ChatDBContext> options)
        : base(options)
    {
    }

    // Your existing domain entities
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Cheep> Cheeps { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Add any custom configurations for your domain entities here if needed
        // For example:
        // builder.Entity<Author>().HasKey(a => a.Id);
        // builder.Entity<Cheep>().HasOne(c => c.Author).WithMany(a => a.Cheeps);
    }
}
