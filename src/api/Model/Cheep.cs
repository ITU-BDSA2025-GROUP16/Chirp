namespace MyChat.Razor.Model;

public class Cheep

{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    // Foreign key + navigation property
    public int UserId { get; set; }
    public Author Author { get; set; }
}