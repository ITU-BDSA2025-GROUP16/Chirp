using Chirp.Core.Domain; 
using Chirp.Core.DTO;

namespace Chirp.Core.Interfaces;
public interface ICheepRepository
{
  /*  Task CreateMessage(MessageDTO newMessage);
    Task<List<MeessageDTO>> ReadMessage(string UserName);
    Task UpdateMessage(MessageDTO alteredMessage);*/

  public List<CheepDTO> GetCheeps(int pageNumber = 1);
  public List<CheepDTO> GetCheepsFromAuthor(string author, int pageNumber = 1);

  public List<CheepDTO> GetCheepsFromFollowedAuthors(List<int> authorIds, int pageNumber = 1);
  public Task CreateCheep(string cheepText, Author author);
  public Task DeleteCheepsByAuthorId(int authorId);

  public List<CheepDTO> GetCheepsByLikes(int pageNumber = 1);
}