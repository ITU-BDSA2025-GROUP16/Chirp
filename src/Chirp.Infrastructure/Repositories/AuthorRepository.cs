using Microsoft.EntityFrameworkCore;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Services;
using Chirp.Core.Domain; 


namespace Chirp.Infrastructure.Repositories;

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

    public Author? GetAuthorFromEmail(string email)
    {
        return _context.Authors
                   .FirstOrDefault(a => a.Email == email);
    }

}   