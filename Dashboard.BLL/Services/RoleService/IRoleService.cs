using Dashboard.DAL.ViewModels;

namespace Dashboard.BLL.Services.RoleService
{
    public interface IRoleService
    {
        Task<ServiceResponse> GetAllAsync();
        Task<ServiceResponse> GetByIdAsync(string id);
        Task<ServiceResponse> GetByNameAsync(string name);
        Task<ServiceResponse> DeleteAsync(string id);
        Task<ServiceResponse> CreteAsync(RoleVM model);
        Task<ServiceResponse> UpdateAsync(RoleVM model);
    }
}
