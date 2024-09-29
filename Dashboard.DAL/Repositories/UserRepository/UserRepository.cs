using Dashboard.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dashboard.DAL.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddToRoleAsync(User model, string role)
        {
            return await _userManager.AddToRoleAsync(model, role);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(User model)
        {
            var result = await _userManager.DeleteAsync(model);
            return result;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email, bool includes = false)
        {
            return await GetUserAsync(u => u.Email == email, includes);
        }

        public async Task<User?> GetByIdAsync(string id, bool includes = false)
        {
            return await GetUserAsync(u => u.Id == id, includes);
        }

        public async Task<User?> GetByUsernameAsync(string userName, bool includes = false)
        {
            return await GetUserAsync(u => u.UserName == userName, includes);
        }

        public async Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate, bool includes = false)
        {
            if (includes)
            {
                return await _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(predicate);
            }
            else
            {
                return await _userManager.Users
                .FirstOrDefaultAsync(predicate);
            }
        }

        public async Task<bool> IsUniqueEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) == null;
        }

        public async Task<bool> IsUniqueUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName) == null;
        }

        public async Task<IdentityResult> UpdateAsync(User model)
        {
            var result = await _userManager.UpdateAsync(model);
            return result;
        }
    }
}
