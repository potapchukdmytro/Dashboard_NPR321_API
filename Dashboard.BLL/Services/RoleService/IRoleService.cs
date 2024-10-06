namespace Dashboard.BLL.Services.RoleService
{
    public interface IRoleService
    {
        Task<ServiceResponse> GetAllAsync();
        Task<ServiceResponse> GetByIdAsync(string id);
        Task<ServiceResponse> GetByNameAsync(string name);
    }
}
