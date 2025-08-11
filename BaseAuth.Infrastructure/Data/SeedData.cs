using BaseAuth.Domain.Entities;
using BaseAuth.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BaseAuth.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Seed Permissions
                await SeedPermissionsAsync(context);

                // Seed Roles
                await SeedRolesAsync(context);

                // Seed Admin User
                await SeedAdminUserAsync(context);

                logger.LogInformation("Seed data completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding data.");
                throw;
            }
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            if (await context.Permissions.AnyAsync())
                return;

            var permissions = new List<Permission>
            {
                // User permissions - Flag enum ile kombinasyonlar
                new Permission { Name = "User Management", Description = "Full user management permissions", Resource = "Users", Type = PermissionType.FullAccess },
                new Permission { Name = "User Read Only", Description = "Can only view users", Resource = "Users", Type = PermissionType.Read },
                new Permission { Name = "User Read Write", Description = "Can view, create and update users", Resource = "Users", Type = PermissionType.ReadWrite },

                // Role permissions
                new Permission { Name = "Role Management", Description = "Full role management permissions", Resource = "Roles", Type = PermissionType.FullAccess },
                new Permission { Name = "Role Read Only", Description = "Can only view roles", Resource = "Roles", Type = PermissionType.Read },
                new Permission { Name = "Role Read Write", Description = "Can view, create and update roles", Resource = "Roles", Type = PermissionType.ReadWrite },

                // Permission permissions
                new Permission { Name = "Permission Management", Description = "Full permission management", Resource = "Permissions", Type = PermissionType.FullAccess },
                new Permission { Name = "Permission Read Only", Description = "Can only view permissions", Resource = "Permissions", Type = PermissionType.Read },

                // Dashboard permissions
                new Permission { Name = "Dashboard Access", Description = "Can access dashboard", Resource = "Dashboard", Type = PermissionType.Read },
                new Permission { Name = "Dashboard Management", Description = "Can manage dashboard settings", Resource = "Dashboard", Type = PermissionType.Manage },

                // Reports permissions
                new Permission { Name = "Reports Access", Description = "Can view and export reports", Resource = "Reports", Type = PermissionType.Read | PermissionType.Export },
                new Permission { Name = "Reports Management", Description = "Full reports management", Resource = "Reports", Type = PermissionType.FullAccess },

                // Settings permissions
                new Permission { Name = "Settings Access", Description = "Can view settings", Resource = "Settings", Type = PermissionType.Read },
                new Permission { Name = "Settings Management", Description = "Can manage all settings", Resource = "Settings", Type = PermissionType.Manage },

                // System permissions
                new Permission { Name = "System Admin", Description = "Full system administration", Resource = "System", Type = PermissionType.AdminAccess },
                new Permission { Name = "System Read Only", Description = "Can view system information", Resource = "System", Type = PermissionType.Read }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return;

            var adminRole = new Role
            {
                Name = "Admin",
                Description = "System administrator with full access",
                IsSystemRole = true
            };

            await context.Roles.AddAsync(adminRole);
            await context.SaveChangesAsync();

            // Get all permissions and assign to admin role
            var allPermissions = await context.Permissions.ToListAsync();
            var adminRolePermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id
            });

            await context.RolePermissions.AddRangeAsync(adminRolePermissions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedAdminUserAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync(u => u.Email == "admin@baseauth.com"))
                return;

            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@baseauth.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Default password
                PhoneNumber = "+905551234567",
                Status = UserStatus.Active,
                EmailConfirmed = true,
                PhoneConfirmed = true,
                CreatedDate = DateTime.UtcNow
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Assign admin role to admin user
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                };

                await context.UserRoles.AddAsync(userRole);
                await context.SaveChangesAsync();
            }
        }
    }
} 