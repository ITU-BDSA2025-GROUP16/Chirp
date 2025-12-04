using Microsoft.EntityFrameworkCore;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Domain;

namespace Chirp.Infrastructure.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly ChatDBContext _context;

    public LikeRepository(ChatDBContext context)
    {
        _context = context;
    }

    public async Task Like(int likerId, int likedCheepId)
    {
        var like = new Like
        {
            LikerId = likerId,
            LikedCheepId = likedCheepId
        };

        _context.Likes.Add(like);
        await _context.SaveChangesAsync();
    }

    public async Task UnLike(int likerId, int likedCheepId)
    {
        var like = await _context.Likes
            .FirstOrDefaultAsync(l => l.LikerId == likerId && l.LikedCheepId == likedCheepId);

        if (like != null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsLiking(int likerId, int likedCheepId)
    {
        return await _context.Likes
            .AnyAsync(l => l.LikerId == likerId && l.LikedCheepId == likedCheepId);
    }

       public async Task<HashSet<int>> GetLikedCheepIds(int likerId)
{
    return await _context.Likes
        .Where(l => l.LikerId == likerId)
        .Select(l => l.LikedCheepId)
        .ToHashSetAsync();
}
    public async Task DeleteLikesByAuthorId(int authorId)
    {
        var likes = _context.Likes.Where(l => l.LikerId == authorId);

        _context.Likes.RemoveRange(likes);

        await _context.SaveChangesAsync();
    }
}