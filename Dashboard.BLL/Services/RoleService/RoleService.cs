  using AutoMapper;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.Repositories.RoleRepository;
using Dashboard.DAL.ViewModels;

namespace Dashboard.BLL.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            var models = _mapper.Map<List<RoleVM>>(roles);

            return ServiceResponse.OkResponse("Ролі отримано", models);
        }

        private async Task<ServiceResponse> GetAsync(Func<string, Task<Role?>> func, string value)
        {
            var role = await func(value);

            if (role == null)
            {
                return ServiceResponse.BadRequestResponse("Не вдалося отримати роль");
            }

            var model = _mapper.Map<RoleVM>(role);

            return ServiceResponse.OkResponse("Роль отримано", model);
        }

        public async Task<ServiceResponse> GetByIdAsync(string id)
        {
            return await GetAsync(_roleRepository.GetByIdAsync, id);
        }

        public async Task<ServiceResponse> GetByNameAsync(string name)
        {
            return await GetAsync(_roleRepository.GetByNameAsync, name);
        }
    }
}
