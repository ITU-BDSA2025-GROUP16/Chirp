
﻿using Microsoft.EntityFrameworkCore;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Services;
using Chirp.Core.Domain; 


namespace Chirp.Infrastructure.Repositories;
public class CheepRepository : ICheepRepository
{

    private readonly ChatDBContext _context;

    public CheepRepository(ChatDBContext context)
    {
        _context = context;
    }
    public List<CheepViewModel> GetCheeps(int pageNumber = 1)
    {
        int limit = 32;
        int offset = (pageNumber - 1) * limit;

        return _context.Cheeps
            .Include(c => c.Author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
                c.Author.Id
            ))
            .ToList();
    }
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1)
    {
        int limit = 32;
        int offset = (pageNumber - 1) * limit;

        return _context.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.Name == author)
            .OrderByDescending(c => c.TimeStamp)
            .Skip(offset)
            .Take(limit)
            .Select(c => new CheepViewModel(
                c.Author.Name,
                c.Text,
                c.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
                c.Author.Id
            ))
            .ToList();
    }

    public List<CheepViewModel> GetCheepsFromFollowedAuthors(List<int> authorIds, int pageNumber = 1)
    {
    const int pageSize = 32;
    
    if (!authorIds.Any())
    {
        return new List<CheepViewModel>();
    }
    
    return _context.Cheeps
        .Where(c => authorIds.Contains(c.AuthorId))
        .OrderByDescending(c => c.TimeStamp)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(c => new CheepViewModel(
            c.Author.Name,
            c.Text,
            c.TimeStamp.ToString("MM/dd/yy H:mm:ss"),
            c.Author.Id
        ))
        .ToList();
    }

   public async Task CreateCheep(string cheepText, Author author)
    {
    //This method assumes that you are logged in, and therefore that the Author already exists!
    
    var cheep = new Cheep
    {
        Text = cheepText,
        TimeStamp = DateTime.UtcNow,
        Author = author
    };

    _context.Cheeps.Add(cheep);
    await _context.SaveChangesAsync(); 
    }

    public async Task DeleteCheepsByAuthorId(int authorId)
    {
        var cheeps = _context.Cheeps.Where(c => c.AuthorId == authorId);

        _context.Cheeps.RemoveRange(cheeps);

        await _context.SaveChangesAsync();
    }


}