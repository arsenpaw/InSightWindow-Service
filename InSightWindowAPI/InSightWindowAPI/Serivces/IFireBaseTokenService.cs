using InSightWindowAPI.Models.Entity;

namespace InSightWindowAPI.Serivces
{
    public interface IFireBaseTokenService
    {
        Task<FireBaseToken> AddNewToken(string token);
        Task AddNewTokenToUser(string token, Guid userId);
        Task AddTokenToUser(Guid userId, Guid tokenId);
    }
}