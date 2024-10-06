using Dashboard.DAL.Models.Identity;
using System.Linq.Expressions;

namespace Dashboard.DAL.Repositories.RoleRepository
{
    public interface IRoleRepository
    {
        Task<Role?> GetAsync(Expression<Func<Role, bool>> predicate);
        Task<Role?> GetByIdAsync(string id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
    }
}
