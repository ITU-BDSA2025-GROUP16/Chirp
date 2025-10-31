using Microsoft.EntityFrameworkCore;
using Chirp.Core.Domain;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;

namespace Chirp.Infrastructure.Data;

public class ChatDBContext : DbContext
{
    public ChatDBContext(DbContextOptions<ChatDBContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Cheep> Cheeps { get; set; } = null!;
}
