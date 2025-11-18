using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.Domain;
public class Follow

{
    public int FollowerId { get; set; } 

    public int FollowedId { get; set; }
}