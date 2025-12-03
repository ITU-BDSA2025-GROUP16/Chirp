using Chirp.Core.Interfaces;
using Chirp.Core.Domain;

namespace Chirp.Core.Services;

public interface IFollowService
{
    public Task Follow(int follower, int followed);
    public Task Unfollow(int follower, int followed);
    public Task<bool> IsFollowing(int follower, int followed); 

    public Task<HashSet<int>> GetFollowedIds(int followerId); //To check who is following
    Task DeleteFollowersData(Author author); 
}

public class FollowService : IFollowService
{
    private readonly IFollowRepository _repo;

    public FollowService(IFollowRepository repository)
    {
        _repo = repository;
    }

    public async Task Follow(int follower, int followed)
    {
        await _repo.Follow(follower, followed);
    }

    public async Task Unfollow(int follower, int followed)
    {
        await _repo.Unfollow(follower, followed);
    }

    public async Task<bool> IsFollowing(int follower, int followed)
    {
        return await _repo.IsFollowing(follower, followed);
    }

    public async Task<HashSet<int>> GetFollowedIds(int followerId)
{
    return await _repo.GetFollowedIds(followerId);
}
    public async Task DeleteFollowersData(Author author)
    {
        await _repo.DeleteFollowersByAuthorId(author.Id);
    }
}