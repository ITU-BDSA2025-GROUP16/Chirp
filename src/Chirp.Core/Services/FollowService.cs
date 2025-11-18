using Chirp.Core.Interfaces;
using Chirp.Core.Domain;

namespace Chirp.Core.Services;

public interface IFollowService
{
    public Task Follow(int follower, int followed);

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


}
