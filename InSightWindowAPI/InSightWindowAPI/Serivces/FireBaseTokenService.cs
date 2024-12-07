using InSightWindowAPI.Exeptions;
using InSightWindowAPI.Models.Entity;
using InSightWindowAPI.Repository;
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
            var isExist = _fireBaseRepository.GetAll().Any(x => x.Token == token);
            if (isExist)
            {
                throw new AppException("Token already exist", HttpStatusCode.Conflict);
            }

            var fireBaseTokenEntity = new FireBaseToken()
            {
                Token = token
            };
            await _fireBaseRepository.AddAsync(fireBaseTokenEntity);
            await _fireBaseRepository.SaveAsync();
            return await _fireBaseRepository.GetByToken(token).FirstAsync();
        }
        public async Task AddTokenToUser(Guid userId, Guid tokenId)
        {
            await _fireBaseRepository.AddTokenToUser(userId, tokenId);
            await _fireBaseRepository.SaveAsync();
        }
        public async Task AddNewTokenToUser(string token, Guid userId)
        {
            //Handle when swich account on same device so token is same
            var existingToken = await _fireBaseRepository.GetByToken(token)
                .SelectMany(x => x.UserFireBaseTokens)
                .FirstOrDefaultAsync();
            if (existingToken is not null)
            {
                existingToken.UserId = userId;
                return;
            }
            var strategy = _fireBaseRepository.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _fireBaseRepository.BeginTransaction();
                var savedFireBaseToken = await AddNewToken(token);
                await AddTokenToUser(userId, savedFireBaseToken.Id);
                await transaction.CommitAsync();
            });
        }
    }
}
