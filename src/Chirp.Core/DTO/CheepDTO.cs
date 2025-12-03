namespace Chirp.Core.DTO;

public record CheepDTO(
    int CheepId,
    int AuthorId,
    string AuthorName,
    string Message,
    DateTime Timestamp,
    int LikeCount
);
