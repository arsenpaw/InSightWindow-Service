using InSightWindowAPI.Models.Entity;

namespace InSightWindowAPI.Serivces.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user, IEnumerable<string> role);
        Task<RefreshToken> GenerateRefreshTokenAsync(User user);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken token);
    }
}
