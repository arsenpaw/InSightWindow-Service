using InSightWindowAPI.Models.Entity;
using InSightWindowAPI.Repository.Interfaces;

namespace InSightWindowAPI.Repository
{
    public interface IFireBaseRepository : IBaseRepository
    {
        public IQueryable<FireBaseToken> GetByToken(string token);
        Task AddAsync(FireBaseToken firebaseToken);
        IQueryable<FireBaseToken> GetAll();
        public void RemoveRelation(UserFireBaseTokens userFireBaseTokens);
        IQueryable<FireBaseToken> GetById(Guid id);
        IQueryable<UserFireBaseTokens> GetByUserId(Guid userId);
        void Remove(FireBaseToken firebaseToken);
        public Task AddTokenToUser(Guid userId, Guid tokenId);
    }
}