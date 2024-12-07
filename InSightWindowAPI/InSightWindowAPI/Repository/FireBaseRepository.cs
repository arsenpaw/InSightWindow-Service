
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.Entity;

namespace InSightWindowAPI.Repository
{
    public class FireBaseRepository : BaseRepository, IFireBaseRepository
    {
        public FireBaseRepository(UsersContext context) : base(context)
        {

        }


        public async Task AddAsync(FireBaseToken firebaseToken) => await _context.FireBaseTokens.AddAsync(firebaseToken);

        public IQueryable<FireBaseToken> GetAll() => _context.FireBaseTokens;

        public IQueryable<FireBaseToken> GetById(Guid id) => _context.FireBaseTokens.Where(x => x.Id == id);

        public IQueryable<UserFireBaseTokens> GetByUserId(Guid userId) => _context.UserFireBaseTokens.Where(x => x.UserId == userId);

        public IQueryable<FireBaseToken> GetByToken(string token) => _context.FireBaseTokens.Where(x => x.Token == token);

        public async Task AddTokenToUser(Guid userId, Guid tokenId)
        {
            var userToken = new UserFireBaseTokens()
            {
                UserId = userId,
                FireBaseTokenId = tokenId
            };
            await _context.UserFireBaseTokens.AddAsync(userToken);
        }
        public void Remove(FireBaseToken firebaseToken) => _context.FireBaseTokens.Remove(firebaseToken);

    }
}
