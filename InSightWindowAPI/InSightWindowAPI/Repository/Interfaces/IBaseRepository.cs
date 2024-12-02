using Microsoft.EntityFrameworkCore;

namespace InSightWindowAPI.Repository.Interfaces
{
    public interface IBaseRepository 
    {
        public Task<int> SaveAsync();

        public int Save();
    }
}
