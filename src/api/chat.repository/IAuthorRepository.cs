namespace MyChat.Razor.chat.repository;
using Microsoft.EntityFrameworkCore;
using MyChat.Razor.Model;

public interface IAuthorRepository
{
    /*  Task CreateMessage(MessageDTO newMessage);
      Task<List<MeessageDTO>> ReadMessage(string UserName);
      Task UpdateMessage(MessageDTO alteredMessage);*/
    public Author? GetAuthorFromName(string name);
  
}