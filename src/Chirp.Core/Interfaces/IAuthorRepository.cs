using Chirp.Core.Domain;
using Chirp.Core.Services;


namespace Chirp.Core.Interfaces;
public interface IAuthorRepository
{
  /*  Task CreateMessage(MessageDTO newMessage);
    Task<List<MeessageDTO>> ReadMessage(string UserName);
    Task UpdateMessage(MessageDTO alteredMessage);*/
  public Author? GetAuthorFromName(string name);

  public Author? GetAuthorFromEmail(string email);
    }
