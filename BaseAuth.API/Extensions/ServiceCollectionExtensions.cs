using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BaseAuth.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add API versioning
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("ver")
                );
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BaseAuth API",
                    Version = "v1",
                    Description = "A base authentication API with JWT, role-based permissions, and user management",
                    Contact = new OpenApiContact
                    {
                        Name = "BaseAuth Team",
                        Email = "admin@BaseAuth.com"
                    }
                });

                // Include XML comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Custom operation filter for better documentation
                c.OperationFilter<SwaggerDefaultValues>();
            });

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
                    
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Add authorization policies
            services.AddAuthorizationPolicies();

            return services;
        }

        private static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Resource-based permission policies
                AddResourcePermissionPolicies(options);
                
                // Role-based policies
                AddRoleBasedPolicies(options);
                
                // Custom policies
                AddCustomPolicies(options);
            });
        }

        private static void AddResourcePermissionPolicies(AuthorizationOptions options)
        {
            // Define resources and their permission types
            var resourcePermissions = new Dictionary<string, string[]>
            {
                ["Users"] = new[] { "Read", "Create", "Update", "Delete", "Manage" },
                ["Roles"] = new[] { "Read", "Create", "Update", "Delete", "Manage" },
                ["Permissions"] = new[] { "Read", "Manage" },
                ["Dashboard"] = new[] { "Read", "Manage" },
                ["Reports"] = new[] { "Read", "Create", "Export", "Manage" },
                ["Settings"] = new[] { "Read", "Update", "Manage" },
                ["System"] = new[] { "Read", "Manage" },
                ["Profile"] = new[] { "Read", "Update" },
                ["Analytics"] = new[] { "Read", "Export" },
                ["Data"] = new[] { "Read", "Export", "Import" }
            };

            // Generate policies for each resource and permission type
            foreach (var resource in resourcePermissions)
            {
                foreach (var permissionType in resource.Value)
                {
                    var policyName = $"Require{resource.Key}{permissionType}Permission";
                    var permissionClaim = $"{resource.Key}.{permissionType}";
                    
                    options.AddPolicy(policyName, policy =>
                        policy.RequireClaim("permission", permissionClaim));
                }
            }

            // Add combined permission policies
            AddCombinedPermissionPolicies(options);
        }

        private static void AddCombinedPermissionPolicies(AuthorizationOptions options)
        {
            // Read-Write policies (Read + Create + Update)
            var readWriteResources = new[] { "Users", "Roles", "Settings" };
            foreach (var resource in readWriteResources)
            {
                options.AddPolicy($"Require{resource}ReadWritePermission", policy =>
                {
                    policy.RequireClaim("permission", $"{resource}.Read");
                    policy.RequireClaim("permission", $"{resource}.Create");
                    policy.RequireClaim("permission", $"{resource}.Update");
                });
            }

            // Full Access policies (Read + Create + Update + Delete + Manage)
            var fullAccessResources = new[] { "Users", "Roles", "Reports", "System" };
            foreach (var resource in fullAccessResources)
            {
                options.AddPolicy($"Require{resource}FullAccessPermission", policy =>
                {
                    policy.RequireClaim("permission", $"{resource}.Read");
                    policy.RequireClaim("permission", $"{resource}.Create");
                    policy.RequireClaim("permission", $"{resource}.Update");
                    policy.RequireClaim("permission", $"{resource}.Delete");
                    policy.RequireClaim("permission", $"{resource}.Manage");
                });
            }
        }

        private static void AddRoleBasedPolicies(AuthorizationOptions options)
        {
            // Admin role requirement
            options.AddPolicy("RequireAdminRole", policy =>
                policy.RequireRole("Admin", "SuperAdmin"));
            
            options.AddPolicy("RequireSuperAdminRole", policy =>
                policy.RequireRole("SuperAdmin"));
            
            options.AddPolicy("RequireManagerRole", policy =>
                policy.RequireRole("Manager", "Admin", "SuperAdmin"));
            
            options.AddPolicy("RequireUserRole", policy =>
                policy.RequireRole("User", "Manager", "Admin", "SuperAdmin"));
        }

        private static void AddCustomPolicies(AuthorizationOptions options)
        {
            // Custom business logic policies
            options.AddPolicy("RequireActiveUser", policy =>
                policy.RequireAssertion(context =>
                {
                    var user = context.User;
                    var isActive = user.HasClaim(c => c.Type == "status" && c.Value == "Active");
                    return isActive;
                }));

            options.AddPolicy("RequireEmailVerified", policy =>
                policy.RequireAssertion(context =>
                {
                    var user = context.User;
                    var emailVerified = user.HasClaim(c => c.Type == "email_verified" && c.Value == "true");
                    return emailVerified;
                }));

            // Time-based policies
            options.AddPolicy("RequireBusinessHours", policy =>
                policy.RequireAssertion(context =>
                {
                    var currentHour = DateTime.UtcNow.Hour;
                    return currentHour >= 9 && currentHour <= 17; // 9 AM - 5 PM UTC
                }));
        }
    }
} 