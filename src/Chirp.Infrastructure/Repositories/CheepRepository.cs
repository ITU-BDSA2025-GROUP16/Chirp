
﻿using Microsoft.EntityFrameworkCore;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Services;
using Chirp.Core.Domain; 
using Chirp.Core.DTO;


namespace Chirp.Infrastructure.Repositories;
public class CheepRepository : ICheepRepository
{

    private readonly ChatDBContext _context;

    public CheepRepository(ChatDBContext context)
    {
        _context = context;
    }
    public List<CheepDTO> GetCheeps(int pageNumber = 1)
    {
        int limit = 32;
        int offset = (pageNumber - 1) * limit;

        var cheeps = _context.Cheeps
        .Include(c => c.Author)
        .OrderByDescending(c => c.TimeStamp)
        .Skip(offset)
        .Take(limit)
        .ToList();

    return cheeps.Select(c => new CheepDTO(
        c.CheepId,
        c.AuthorId,
        c.Author.UserName,
        c.Text,
        c.TimeStamp,
        _context.Likes.Count(l => l.LikedCheepId == c.CheepId)
    )).ToList();
    }
    public List<CheepDTO> GetCheepsFromAuthor(string author, int pageNumber = 1)
    {
        int limit = 32;
        int offset = (pageNumber - 1) * limit;

        var cheeps = _context.Cheeps
        .Include(c => c.Author)
        .Where(c => c.Author.UserName == author)
        .OrderByDescending(c => c.TimeStamp)
        .Skip(offset)
        .Take(limit)
        .ToList();

       return cheeps.Select(c => new CheepDTO(
        c.CheepId,
        c.AuthorId,
        c.Author.UserName,
        c.Text,
        c.TimeStamp,
        _context.Likes.Count(l => l.LikedCheepId == c.CheepId)
    )).ToList();
    }

    public List<CheepDTO> GetCheepsFromFollowedAuthors(List<int> authorIds, int pageNumber = 1)
    {
    const int pageSize = 32;
    
    if (!authorIds.Any())
    {
        return new List<CheepDTO>();
    }

    var cheeps = _context.Cheeps
    .Include(c => c.Author)
    .Where(c => authorIds.Contains(c.AuthorId))
    .OrderByDescending(c => c.TimeStamp)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();

    
    return cheeps.Select(c => new CheepDTO(
        c.CheepId,
        c.AuthorId,
        c.Author != null ? c.Author.UserName : "Unknown",
        c.Text,
        c.TimeStamp,
        _context.Likes.Count(l => l.LikedCheepId == c.CheepId)
    )).ToList();
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

public List<CheepDTO> GetCheepsByLikes(int pageNumber = 1)
{
    int pageSize = 32;
    int offset = (pageNumber - 1) * pageSize;

    var cheeps = _context.Cheeps
        .Include(c => c.Author)
        .AsEnumerable()
        .Select(c => new CheepDTO(
            c.CheepId,
            c.AuthorId,
            c.Author != null ? c.Author.UserName : "Unknown",
            c.Text,
            c.TimeStamp,
            _context.Likes.Count(l => l.LikedCheepId == c.CheepId)
        ))
        .OrderByDescending(c => c.LikeCount)
        .ThenByDescending(c => c.Timestamp)
        .Skip(offset)
        .Take(pageSize)
        .ToList();

    return cheeps;
}
}