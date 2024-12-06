using InSightWindowAPI.Models;
using InSightWindowAPI.Repository.Interfaces;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.EntityFrameworkCore;

namespace InSightWindowAPI.Repository
{
    public class UserRepository: BaseRepository,IUserRepository
    {
        public UserRepository(UsersContext context) : base(context)
        {
            
        }

        public IQueryable<User> GetAll()
            => _context.Users;

        public IQueryable<User> GetById(Guid userId) => _context.Users.Where(x => x.Id == userId);

    }
}
