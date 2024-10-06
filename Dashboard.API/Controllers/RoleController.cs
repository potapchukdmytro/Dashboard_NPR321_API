using Dashboard.BLL.Services;
using Dashboard.BLL.Services.RoleService;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string? id, string? name)
        {
            id = Request.Query[nameof(id)];
            name = Request.Query[nameof(name)];

            if (id == null && name == null)
            {
                var response = await _roleService.GetAllAsync();
                return GetResult(response);
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                var response = await _roleService.GetByIdAsync(id);

                if(response.Success)
                {
                    return GetResult(response);
                }
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                var response = await _roleService.GetByNameAsync(name);

                if (response.Success)
                {
                    return GetResult(response);
                }
            }

            return GetResult(ServiceResponse.BadRequestResponse("Не вдалося отримати роль"));
        }
    }
}
