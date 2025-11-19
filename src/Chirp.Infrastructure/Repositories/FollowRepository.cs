using Microsoft.EntityFrameworkCore;
using Chirp.Core.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Services;
using Chirp.Core.Domain; 


namespace Chirp.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly ChatDBContext _context;

    public FollowRepository(ChatDBContext context)
    {
        _context = context;
    }

    public async Task Follow(int followerId, int followedId)
    {
        var follow = new Follow 
        { 
            FollowerId = followerId, 
            FollowedId = followedId 
        };
        
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();
    }

    public async Task Unfollow(int followerId, int followedId)
    {
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        
        if (follow != null)
        {
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsFollowing(int followerId, int followedId)
    {
        return await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
    }
} 