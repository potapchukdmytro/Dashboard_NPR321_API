using Dashboard.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.DAL.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string userName);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<bool> IsUniqueEmailAsync(string email);
        Task<bool> IsUniqueUserNameAsync(string userName);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    }
}
