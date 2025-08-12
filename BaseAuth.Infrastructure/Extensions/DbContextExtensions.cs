using BaseAuth.Domain.Entities;
using BaseAuth.Domain.Enums;
using BaseAuth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BaseAuth.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task SeedDataAsync(this ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            try
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
                
                logger.LogInformation("Starting database seeding...");

                // Seed Permissions
                await SeedPermissionsAsync(context, logger);
                
                // Seed Roles
                await SeedRolesAsync(context, logger);
                
                // Seed Default Admin User
                await SeedAdminUserAsync(context, serviceProvider, logger);

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Permissions.AnyAsync())
            {
                logger.LogInformation("Permissions already exist, skipping permission seeding.");
                return;
            }

            var permissions = new[]
            {
                // User permissions
                new Permission { Name = "Users.Create", Description = "Create users", Resource = "Users", Type = PermissionType.Create },
                new Permission { Name = "Users.Read", Description = "View users", Resource = "Users", Type = PermissionType.Read },
                new Permission { Name = "Users.Update", Description = "Update users", Resource = "Users", Type = PermissionType.Update },
                new Permission { Name = "Users.Delete", Description = "Delete users", Resource = "Users", Type = PermissionType.Delete },

                // Role permissions
                new Permission { Name = "Roles.Create", Description = "Create roles", Resource = "Roles", Type = PermissionType.Create },
                new Permission { Name = "Roles.Read", Description = "View roles", Resource = "Roles", Type = PermissionType.Read },
                new Permission { Name = "Roles.Update", Description = "Update roles", Resource = "Roles", Type = PermissionType.Update },
                new Permission { Name = "Roles.Delete", Description = "Delete roles", Resource = "Roles", Type = PermissionType.Delete },

                // Permission permissions
                new Permission { Name = "Permissions.Read", Description = "View permissions", Resource = "Permissions", Type = PermissionType.Read },
                new Permission { Name = "Permissions.Create", Description = "Create permissions", Resource = "Permissions", Type = PermissionType.Create },
                new Permission { Name = "Permissions.Update", Description = "Update permissions", Resource = "Permissions", Type = PermissionType.Update },
                new Permission { Name = "Permissions.Delete", Description = "Delete permissions", Resource = "Permissions", Type = PermissionType.Delete },

                // Dashboard permissions
                new Permission { Name = "Dashboard.Read", Description = "View dashboard", Resource = "Dashboard", Type = PermissionType.Read },
                new Permission { Name = "Dashboard.Create", Description = "Create dashboard", Resource = "Dashboard", Type = PermissionType.Create },
                new Permission { Name = "Dashboard.Update", Description = "Update dashboard", Resource = "Dashboard", Type = PermissionType.Update },
                new Permission { Name = "Dashboard.Delete", Description = "Delete dashboard", Resource = "Dashboard", Type = PermissionType.Delete },

                // Reports permissions
                new Permission { Name = "Reports.Read", Description = "View reports", Resource = "Reports", Type = PermissionType.Read },
                new Permission { Name = "Reports.Create", Description = "Create reports", Resource = "Reports", Type = PermissionType.Create },
                new Permission { Name = "Reports.Update", Description = "Update reports", Resource = "Reports", Type = PermissionType.Update },
                new Permission { Name = "Reports.Delete", Description = "Delete reports", Resource = "Reports", Type = PermissionType.Delete }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {PermissionCount} permissions.", permissions.Length);
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Roles.AnyAsync())
            {
                logger.LogInformation("Roles already exist, skipping role seeding.");
                return;
            }

            var allPermissions = await context.Permissions.ToListAsync();

            // Create SuperAdmin role with all permissions
            var superAdminRole = new Role
            {
                Name = "SuperAdmin",
                Description = "Super Administrator with full system access",
                IsSystemRole = true
            };

            await context.Roles.AddAsync(superAdminRole);
            await context.SaveChangesAsync();

            // Assign all permissions to SuperAdmin
            var superAdminPermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = p.Id
            });

            await context.RolePermissions.AddRangeAsync(superAdminPermissions);

            // Create Admin role with most permissions
            var adminRole = new Role
            {
                Name = "Admin",
                Description = "Administrator with most system access",
                IsSystemRole = true
            };

            await context.Roles.AddAsync(adminRole);
            await context.SaveChangesAsync();

            // Assign permissions to Admin (exclude some manage permissions)
            var adminPermissionNames = new[]
            {
                "Users.Create", "Users.Read", "Users.Update", "Users.Delete",
                "Roles.Read", "Roles.Update",
                "Permissions.Read",
                "Dashboard.Read", "Dashboard.Create", "Dashboard.Update",
                "Reports.Read", "Reports.Create"
            };

            var adminPermissions = allPermissions
                .Where(p => adminPermissionNames.Contains(p.Name))
                .Select(p => new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = p.Id
                });

            await context.RolePermissions.AddRangeAsync(adminPermissions);

            // Create User role with basic permissions
            var userRole = new Role
            {
                Name = "User",
                Description = "Basic user with limited access",
                IsSystemRole = true
            };

            await context.Roles.AddAsync(userRole);
            await context.SaveChangesAsync();

            // Assign basic permissions to User
            var userPermissionNames = new[]
            {
                "Dashboard.Read",
                "Reports.Read"
            };

            var userPermissions = allPermissions
                .Where(p => userPermissionNames.Contains(p.Name))
                .Select(p => new RolePermission
                {
                    RoleId = userRole.Id,
                    PermissionId = p.Id
                });

            await context.RolePermissions.AddRangeAsync(userPermissions);

            await context.SaveChangesAsync();
            logger.LogInformation("Seeded 3 default roles: SuperAdmin, Admin, User.");
        }

        private static async Task SeedAdminUserAsync(ApplicationDbContext context, IServiceProvider serviceProvider, ILogger logger)
        {
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist, skipping admin user seeding.");
                return;
            }

            var passwordService = serviceProvider.GetRequiredService<BaseAuth.Application.Services.IPasswordService>();
            var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

            if (superAdminRole == null)
            {
                logger.LogWarning("SuperAdmin role not found, cannot create admin user.");
                return;
            }

            var defaultPassword = "Admin123!";
            var hashedPasswordResult = passwordService.HashPassword(defaultPassword);

            if (!hashedPasswordResult.IsSuccess)
            {
                logger.LogError("Failed to hash admin password: {Error}", hashedPasswordResult.Error);
                return;
            }

            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@BaseAuth.com",
                PasswordHash = hashedPasswordResult.Data,
                Status = UserStatus.Active,
                EmailConfirmed = true,
                PhoneConfirmed = false
            };

            await context.Users.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Assign SuperAdmin role to admin user
            var userRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = superAdminRole.Id
            };

            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();

            logger.LogInformation("Created default admin user. Email: {Email}, Password: {Password}", 
                adminUser.Email, defaultPassword);
        }

        public static async Task EnsureDatabaseCreatedAsync(this ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task ApplyMigrationsAsync(this ApplicationDbContext context)
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }
    }
} 