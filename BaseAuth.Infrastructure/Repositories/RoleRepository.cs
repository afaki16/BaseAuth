using BaseAuth.Application.Interfaces;
using BaseAuth.Domain.Entities;
using BaseAuth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAuth.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<Role> GetRoleWithPermissionsAsync(Guid roleId)
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<bool> RoleExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(r => r.Name == name);
        }

        public async Task<bool> RoleHasPermissionAsync(Guid roleId, string permission)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .AnyAsync(rp => rp.RoleId == roleId && rp.Permission.Name == permission);
        }

        public async Task<IEnumerable<string>> GetRolePermissionsAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission.Name)
                .ToListAsync();
        }

        public async Task<RolePermission> GetRolePermissionAsync(Guid roleId, Guid permissionId)
        {
            return await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }

        public async Task AddRolePermissionAsync(RolePermission rolePermission)
        {
            await _context.RolePermissions.AddAsync(rolePermission);
        }

        public void RemoveRolePermission(RolePermission rolePermission)
        {
            _context.RolePermissions.Remove(rolePermission);
        }
    }
} 