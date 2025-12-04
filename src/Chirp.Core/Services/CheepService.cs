using Chirp.Core.Interfaces;
using Chirp.Core.Domain;
using Chirp.Core.DTO;

namespace Chirp.Core.Services;

public record CheepViewModel(string Author, string Message, string Timestamp, int AuthorId, int CheepId, int LikeCount);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNumber = 1);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1);

    public List<CheepViewModel> GetCheepsFromFollowedAuthors(List<int> authorIds, int pageNumber = 1); //A little akward but so that we can get followers cheeps from ID

    public Task CreateCheep(Author author, string text);
    Task DeleteUserData(Author author);

    public List<CheepViewModel> GetCheepsByLikes(int pageNumber = 1);

}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repo;

    public CheepService(ICheepRepository repository)
    {
        _repo = repository;
    }


    public async Task CreateCheep(Author author, string text)
    {
        await _repo.CreateCheep(text, author);
    }

    // These would normally be loaded from a database for example
    public List<CheepViewModel> GetCheeps(int pageNumber = 1)
    {
        var dtos = _repo.GetCheeps(pageNumber);

    return dtos.Select(dto => new CheepViewModel(
        dto.AuthorName,
        dto.Message,
        dto.Timestamp.ToString("g"),
        dto.AuthorId,
        dto.CheepId,
        dto.LikeCount
    )).ToList();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1)
    {
        var dtos = _repo.GetCheepsFromAuthor(author, pageNumber);

    return dtos.Select(dto => new CheepViewModel(
        dto.AuthorName,
        dto.Message,
        dto.Timestamp.ToString("g"),
        dto.AuthorId,
        dto.CheepId,
        dto.LikeCount
    )).ToList();
    }

    public List<CheepViewModel> GetCheepsFromFollowedAuthors(List<int> authorIds, int pageNumber = 1)
{
    var dtos = _repo.GetCheepsFromFollowedAuthors(authorIds, pageNumber);

    return dtos.Select(dto => new CheepViewModel(
        dto.AuthorName,
        dto.Message,
        dto.Timestamp.ToString("g"),
        dto.AuthorId,
        dto.CheepId,
        dto.LikeCount
    )).ToList();
}

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    public async Task DeleteUserData(Author author)
    {
        await _repo.DeleteCheepsByAuthorId(author.Id);
    }

public List<CheepViewModel> GetCheepsByLikes(int pageNumber = 1)
{
     var dtos = _repo.GetCheepsByLikes(pageNumber);

    return dtos.Select(dto => new CheepViewModel(
        dto.AuthorName,
        dto.Message,
        dto.Timestamp.ToString("g"),
        dto.AuthorId,
        dto.CheepId,
        dto.LikeCount
    )).ToList();
}
}
