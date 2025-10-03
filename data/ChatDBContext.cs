using Microsoft.EntityFrameworkCore;
using MyChat.Razor.Model;

namespace MyChat.Razor.data
{
    public class ChatDBContext : DbContext
    {
        public ChatDBContext(DbContextOptions<ChatDBContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}