using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Domain;
public class Cheep

{
    public int CheepId { get; set; }

    [Required]
    [MaxLength(160)]
    public string Text { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }

    // Foreign key + navigation property
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}