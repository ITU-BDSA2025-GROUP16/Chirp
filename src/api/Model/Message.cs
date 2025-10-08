namespace MyChat.Razor.Model;
public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    // Foreign key + navigation property
    public int UserId { get; set; }
    public User User { get; set; }
}