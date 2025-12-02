using Chirp.Core.Domain;
using Chirp.Core.Services;


namespace Chirp.Core.Interfaces;
public interface ILikeRepository
{
    public Task Like(int follower, int followed);
    public Task UnLike(int follower, int followed);
    public Task<bool> IsLiking(int follower, int followed);

    //public Task<HashSet<int>> GetFollowedIds(int followerId);
}
