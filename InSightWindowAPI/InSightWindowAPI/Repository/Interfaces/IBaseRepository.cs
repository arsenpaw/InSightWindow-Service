using Microsoft.EntityFrameworkCore.Storage;

namespace InSightWindowAPI.Repository.Interfaces
{
    public interface IBaseRepository
    {
        public IExecutionStrategy CreateExecutionStrategy();

        public Task<int> SaveAsync();

        public int Save();

        public Task<IDbContextTransaction> BeginTransaction();

        public Task CommitTransaction();
    }
}
