using Chirp.Core.Domain;
using Chirp.Core.Services;


namespace Chirp.Core.Interfaces;
public interface IFollowRepository
{
  
  public Task Follow(int follower, int followed);
  }
