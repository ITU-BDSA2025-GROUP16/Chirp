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

     public async Task Follow(int follower, int followed)
    {
        
        var follow = new Follow
        {
        FollowerId = follower,
        FollowedId = followed
        };

        _context.Follows.Add(follow);
    await _context.SaveChangesAsync(); 
    }

}   