namespace MyChat.Razor.chat.repository;
using Microsoft.EntityFrameworkCore;

public interface iCheepRepository
{
  /*  Task CreateMessage(MessageDTO newMessage);
    Task<List<MeessageDTO>> ReadMessage(string UserName);
    Task UpdateMessage(MessageDTO alteredMessage);*/

    public List<CheepViewModel> GetCheeps(int pageNumber = 1);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1);

}