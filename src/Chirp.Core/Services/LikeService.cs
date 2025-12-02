using Chirp.Core.Interfaces;
using Chirp.Core.Domain;

namespace Chirp.Core.Services;

public interface ILikeService
{
    public Task Like(int Liker, int Liked);
    public Task UnLike(int Liker, int Liked);
    public Task<bool> IsLiking(int Liker, int Liked); 

    //public Task<HashSet<int>> GetFollowedIds(int followerId); //To check who is following
}

public class LikeService : ILikeService
{
    private readonly ILikeRepository _repo;

    public LikeService(ILikeRepository repository)
    {
        _repo = repository;
    }

    public async Task Like(int Liker, int Liked)
    {
        await _repo.Like(Liker, Liked);
    }

    public async Task UnLike(int Liker, int Liked)
    {
        await _repo.UnLike(Liker, Liked);
    }

    public async Task<bool> IsLiking(int Liker, int Liked)
    {
        return await _repo.IsLiking(Liker, Liked);
    }

    /*public async Task<HashSet<int>> GetFollowedIds(int followerId)
{
    return await _repo.GetFollowedIds(followerId);
}*/
}