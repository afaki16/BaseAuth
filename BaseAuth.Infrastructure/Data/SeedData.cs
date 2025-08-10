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
                // User permissions
                new Permission { Name = "Create User", Description = "Can create new users", Resource = "Users", Type = PermissionType.Create },
                new Permission { Name = "Read User", Description = "Can view user information", Resource = "Users", Type = PermissionType.Read },
                new Permission { Name = "Update User", Description = "Can update user information", Resource = "Users", Type = PermissionType.Update },
                new Permission { Name = "Delete User", Description = "Can delete users", Resource = "Users", Type = PermissionType.Delete },
                new Permission { Name = "Manage User", Description = "Full user management", Resource = "Users", Type = PermissionType.Manage },

                // Role permissions
                new Permission { Name = "Create Role", Description = "Can create new roles", Resource = "Roles", Type = PermissionType.Create },
                new Permission { Name = "Read Role", Description = "Can view role information", Resource = "Roles", Type = PermissionType.Read },
                new Permission { Name = "Update Role", Description = "Can update role information", Resource = "Roles", Type = PermissionType.Update },
                new Permission { Name = "Delete Role", Description = "Can delete roles", Resource = "Roles", Type = PermissionType.Delete },
                new Permission { Name = "Manage Role", Description = "Full role management", Resource = "Roles", Type = PermissionType.Manage },

                // Permission permissions
                new Permission { Name = "Create Permission", Description = "Can create new permissions", Resource = "Permissions", Type = PermissionType.Create },
                new Permission { Name = "Read Permission", Description = "Can view permission information", Resource = "Permissions", Type = PermissionType.Read },
                new Permission { Name = "Update Permission", Description = "Can update permission information", Resource = "Permissions", Type = PermissionType.Update },
                new Permission { Name = "Delete Permission", Description = "Can delete permissions", Resource = "Permissions", Type = PermissionType.Delete },
                new Permission { Name = "Manage Permission", Description = "Full permission management", Resource = "Permissions", Type = PermissionType.Manage },

                // System permissions
                new Permission { Name = "System Admin", Description = "Full system administration", Resource = "System", Type = PermissionType.Manage },
                new Permission { Name = "View Logs", Description = "Can view system logs", Resource = "System", Type = PermissionType.Read },
                new Permission { Name = "Manage Settings", Description = "Can manage system settings", Resource = "System", Type = PermissionType.Update }
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