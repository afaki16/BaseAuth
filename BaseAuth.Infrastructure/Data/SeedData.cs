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
                // Clear existing data if needed
                await ClearExistingDataAsync(context, logger);

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

        public static async Task SeedAsyncIfEmpty(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Check if database has any data
                var hasAnyData = await context.Users.AnyAsync() || 
                                await context.Roles.AnyAsync() || 
                                await context.Permissions.AnyAsync();

                if (hasAnyData)
                {
                    logger.LogInformation("Database already contains data. Skipping seed data.");
                    return;
                }

                logger.LogInformation("Database is empty. Starting seed data process...");

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

        private static async Task ClearExistingDataAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Clearing existing data...");

                // Clear in correct order to avoid foreign key constraints
                context.UserRoles.RemoveRange(context.UserRoles);
                context.RolePermissions.RemoveRange(context.RolePermissions);
                context.Users.RemoveRange(context.Users);
                context.Roles.RemoveRange(context.Roles);
                context.Permissions.RemoveRange(context.Permissions);
                context.RefreshTokens.RemoveRange(context.RefreshTokens);

                await context.SaveChangesAsync();
                logger.LogInformation("Existing data cleared successfully.");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error clearing existing data, continuing with seed...");
            }
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            if (await context.Permissions.AnyAsync())
                return;

            var permissions = new List<Permission>
            {
                // User permissions - Controller policy'lerine uygun
                new Permission { Name = "Users Read", Description = "Can view users", Resource = "Users", Type = PermissionType.Read },
                new Permission { Name = "Users Create", Description = "Can create users", Resource = "Users", Type = PermissionType.Create },
                new Permission { Name = "Users Update", Description = "Can update users", Resource = "Users", Type = PermissionType.Update },
                new Permission { Name = "Users Delete", Description = "Can delete users", Resource = "Users", Type = PermissionType.Delete },
                new Permission { Name = "Users Manage", Description = "Can manage users", Resource = "Users", Type = PermissionType.Manage },

                // Role permissions - Controller policy'lerine uygun
                new Permission { Name = "Roles Read", Description = "Can view roles", Resource = "Roles", Type = PermissionType.Read },
                new Permission { Name = "Roles Create", Description = "Can create roles", Resource = "Roles", Type = PermissionType.Create },
                new Permission { Name = "Roles Update", Description = "Can update roles", Resource = "Roles", Type = PermissionType.Update },
                new Permission { Name = "Roles Delete", Description = "Can delete roles", Resource = "Roles", Type = PermissionType.Delete },
                new Permission { Name = "Roles Manage", Description = "Can manage roles", Resource = "Roles", Type = PermissionType.Manage },

                // Permission permissions
                new Permission { Name = "Permissions Read", Description = "Can view permissions", Resource = "Permissions", Type = PermissionType.Read },
                new Permission { Name = "Permissions Manage", Description = "Can manage permissions", Resource = "Permissions", Type = PermissionType.Manage },

                // Dashboard permissions
                new Permission { Name = "Dashboard Read", Description = "Can access dashboard", Resource = "Dashboard", Type = PermissionType.Read },
                new Permission { Name = "Dashboard Manage", Description = "Can manage dashboard settings", Resource = "Dashboard", Type = PermissionType.Manage },

                // Reports permissions
                new Permission { Name = "Reports Read", Description = "Can view reports", Resource = "Reports", Type = PermissionType.Read },
                new Permission { Name = "Reports Create", Description = "Can create reports", Resource = "Reports", Type = PermissionType.Create },
                new Permission { Name = "Reports Export", Description = "Can export reports", Resource = "Reports", Type = PermissionType.Export },
                new Permission { Name = "Reports Manage", Description = "Can manage reports", Resource = "Reports", Type = PermissionType.Manage },

                // Settings permissions
                new Permission { Name = "Settings Read", Description = "Can view settings", Resource = "Settings", Type = PermissionType.Read },
                new Permission { Name = "Settings Update", Description = "Can update settings", Resource = "Settings", Type = PermissionType.Update },
                new Permission { Name = "Settings Manage", Description = "Can manage all settings", Resource = "Settings", Type = PermissionType.Manage },

                // System permissions
                new Permission { Name = "System Read", Description = "Can view system information", Resource = "System", Type = PermissionType.Read },
                new Permission { Name = "System Manage", Description = "Full system administration", Resource = "System", Type = PermissionType.Manage }
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