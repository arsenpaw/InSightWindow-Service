using InSightWindowAPI.Exeptions;
using InSightWindowAPI.Models.Entity;
using InSightWindowAPI.Repository;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InSightWindowAPI.Serivces
{
    public class FireBaseTokenService : IFireBaseTokenService
    {
        private readonly IFireBaseRepository _fireBaseRepository;

        public FireBaseTokenService(IFireBaseRepository fireBaseRepository)
        {
            _fireBaseRepository = fireBaseRepository;
        }

        public async Task<FireBaseToken> AddNewToken(string token)
        {
            if (_fireBaseRepository.GetAll().Any(x => x.Token == token))
            {
                throw new AppException("Token already exists", HttpStatusCode.Conflict);
            }

            var fireBaseToken = new FireBaseToken { Token = token };
            await _fireBaseRepository.AddAsync(fireBaseToken);
            await _fireBaseRepository.SaveAsync();
            return fireBaseToken;
        }

        public async Task AddExistingTokenToUser(Guid userId, Guid tokenId)
        {
            await _fireBaseRepository.AddTokenToUser(userId, tokenId);
            await _fireBaseRepository.SaveAsync();
        }

        public async Task AddNewTokenToUser(string token, Guid userId)
        {
            var existingToken = await GetExistingToken(token);
            if (existingToken != null)
            {
                await BindExistingUserToExistingToken(userId, existingToken);
            }
            else
            {
                await AddNewTokenToUserTransaction(token, userId);
            }
        }

        public async Task RemoveDeviceAndUserConnection(Guid userId, string token)
        {
            var userTokens = await _fireBaseRepository.GetByUserId(userId)
                .Where(x => x.FireBaseToken.Token == token)
                .ToArrayAsync();
            _fireBaseRepository.RemoveManyRelations(userTokens);
            await _fireBaseRepository.SaveAsync();
        }

        public async Task<IEnumerable<UserFireBaseTokens>> GetUserTokens(Guid userId) =>
            await _fireBaseRepository.GetByUserId(userId).ToListAsync();

        private async Task<UserFireBaseTokens?> GetExistingToken(string token)
        {
            return await _fireBaseRepository.GetByToken(token)
                .SelectMany(x => x.UserFireBaseTokens)
                .Include(x => x.User)
                .Include(x => x.FireBaseToken)
                .FirstOrDefaultAsync();
        }

        private async Task AddNewTokenToUserTransaction(string token, Guid userId)
        {
            var strategy = _fireBaseRepository.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _fireBaseRepository.BeginTransaction();
                var newFireBaseToken = await AddNewToken(token);
                await AddExistingTokenToUser(userId, newFireBaseToken.Id);
                await transaction.CommitAsync();
            });
        }

        private async Task BindExistingUserToExistingToken(Guid userId, UserFireBaseTokens token)
        {
            var strategy = _fireBaseRepository.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _fireBaseRepository.BeginTransaction();
                _fireBaseRepository.RemoveRelation(token);
                await _fireBaseRepository.SaveAsync();
                await AddExistingTokenToUser(userId, token.FireBaseTokenId);
                await _fireBaseRepository.SaveAsync();
                await transaction.CommitAsync();
            });
        }
    }
}
