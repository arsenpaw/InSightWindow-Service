using InSightWindowAPI.Models;
using InSightWindowAPI.Repository.Interfaces;

namespace InSightWindowAPI.Repository
{
    public abstract class BaseRepository : IBaseRepository
    {
        protected readonly UsersContext _context;

        public BaseRepository(UsersContext context)
        {
            _context = context;
        }

        public Task<int> SaveAsync() => _context.SaveChangesAsync();

        public int Save() => _context.SaveChanges();
    }
}
