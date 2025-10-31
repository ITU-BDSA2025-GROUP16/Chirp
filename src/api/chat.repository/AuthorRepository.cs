namespace MyChat.Razor.chat.repository;
using Microsoft.EntityFrameworkCore;
using MyChat.Razor.data;
using MyChat.Razor.Model;

public class AuthorRepository : IAuthorRepository
{

    private readonly ChatDBContext _context;

    public AuthorRepository(ChatDBContext context)
    {
        _context = context;
    }


    public Author? GetAuthorFromName(string name)
    {
        return _context.Authors
                   .FirstOrDefault(a => a.Name == name);
    }
    

}   