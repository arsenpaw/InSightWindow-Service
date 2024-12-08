using InSightWindowAPI.Models.Entity;

namespace InSightWindowAPI.Serivces.Interfaces
{
    public interface IFireBaseTokenService
    {
        Task<FireBaseToken> AddNewToken(string token);
        Task AddNewTokenToUser(string token, Guid userId);
        Task AddExistingTokenToUser(Guid userId, Guid tokenId);
        Task RemoveDeviceAndUserConnection(Guid userId, string token);
        Task<IEnumerable<UserFireBaseTokens>> GetUserTokens(Guid userId);
    }
}