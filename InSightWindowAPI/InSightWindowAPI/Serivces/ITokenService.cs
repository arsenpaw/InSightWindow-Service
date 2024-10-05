using InSightWindowAPI.Models;

namespace InSightWindowAPI.Serivces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync(User user);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken token);
    }
}
