using AutoMapper;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.ViewModels;

namespace Dashboard.BLL.MappingProfiles
{
    public class RoleMapperProfile : Profile
    {
        public RoleMapperProfile() 
        {
            // Role -> RoleVM
            CreateMap<Role, RoleVM>();

            // RoleVM -> Role
            CreateMap<RoleVM, Role>();
        }
    }
}
