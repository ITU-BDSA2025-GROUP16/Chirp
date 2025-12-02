using Chirp.Core.Services;
using Chirp.Core.Domain; 


namespace Chirp.Core.Interfaces;
public interface ICheepRepository
{
  /*  Task CreateMessage(MessageDTO newMessage);
    Task<List<MeessageDTO>> ReadMessage(string UserName);
    Task UpdateMessage(MessageDTO alteredMessage);*/

  public List<CheepViewModel> GetCheeps(int pageNumber = 1);
  public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1);

  public List<CheepViewModel> GetCheepsFromFollowedAuthors(List<int> authorIds, int pageNumber = 1);
  public Task CreateCheep(string cheepText, Author author);
  public Task DeleteCheepsByAuthorId(int authorId);

  public List<CheepViewModel> GetCheepsByLikes(int pageNumber = 1);
}