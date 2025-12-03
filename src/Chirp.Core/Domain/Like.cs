using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Domain;
public class Like

{
    public int LikerId { get; set; } 

    public int LikedCheepId { get; set; }
}