using Chirp.Core.Domain;
using Chirp.Core.Services;


namespace Chirp.Core.Interfaces;
public interface IFollowRepository
{
    public Task Follow(int follower, int followed);
    public Task Unfollow(int follower, int followed);
    public Task<bool> IsFollowing(int follower, int followed);

    public Task<HashSet<int>> GetFollowedIds(int followerId);

    public Task DeleteFollowersByAuthorId(int authorId);
}
