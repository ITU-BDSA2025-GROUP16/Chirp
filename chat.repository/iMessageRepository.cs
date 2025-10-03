namespace MyChat.Razor.chat.repository;

public interface iMessageRepository
{
    Task CreateMessage(MessageDTO newMessage);
    Task<List<MeessageDTO>> ReadMessage(string UserName);
    Task UpdateMessage(MessageDTO alteredMessage);
    

}