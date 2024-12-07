using InSightWindowAPI.Models;
using InSightWindowAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace InSightWindowAPI.Repository
{
    public abstract class BaseRepository : IBaseRepository
    {
        protected readonly UsersContext _context;

        public BaseRepository(UsersContext context)
        {
            _context = context;
        }
        public IExecutionStrategy CreateExecutionStrategy() => _context.Database.CreateExecutionStrategy();

        public Task<int> SaveAsync() => _context.SaveChangesAsync();

        public int Save() => _context.SaveChanges();

        public Task<IDbContextTransaction> BeginTransaction() => _context.Database.BeginTransactionAsync();

        public Task CommitTransaction() => _context.Database.CommitTransactionAsync();
    }
}
