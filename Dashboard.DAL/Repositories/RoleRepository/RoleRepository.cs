﻿using Dashboard.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dashboard.DAL.Repositories.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<Role?> GetAsync(Expression<Func<Role, bool>> predicate)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(predicate);
            return role;
        }

        public async Task<Role?> GetByIdAsync(string id)
        {
            return await GetAsync(r => r.Id == id);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await GetAsync(r => r.Name == name);
        }
    }
}
