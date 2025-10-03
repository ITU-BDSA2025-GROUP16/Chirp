namespace MyChat.Razor.Model;

public class User
{
    public int Id { get; set; }  // EF Core will consider this the primary key by convention
    public string Username { get; set; }
    public string Email { get; set; }

    // Navigation property
    public ICollection<Message> Messages { get; set; }
}