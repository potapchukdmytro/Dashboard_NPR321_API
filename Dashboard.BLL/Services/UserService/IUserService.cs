using Dashboard.DAL.ViewModels;

namespace Dashboard.BLL.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse> GetById(string id);
        Task<ServiceResponse> GetByEmail(string email);
        Task<ServiceResponse> GetByUserName(string userName);
        Task<ServiceResponse> GetAllAsync();
        Task<ServiceResponse> DeleteAsync(string id);
        Task<ServiceResponse> CreateAsync(CreateUpdateUserVM model);
        Task<ServiceResponse> UpdateAsync(CreateUpdateUserVM model);
    }
}
