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
            services.AddAuthorization(options =>
            {
                // User permissions
                options.AddPolicy("RequireUsersReadPermission", policy =>
                    policy.RequireClaim("permission", "Users.Read"));
                options.AddPolicy("RequireUsersCreatePermission", policy =>
                    policy.RequireClaim("permission", "Users.Create"));
                options.AddPolicy("RequireUsersUpdatePermission", policy =>
                    policy.RequireClaim("permission", "Users.Update"));
                options.AddPolicy("RequireUsersDeletePermission", policy =>
                    policy.RequireClaim("permission", "Users.Delete"));
                options.AddPolicy("RequireUsersManagePermission", policy =>
                    policy.RequireClaim("permission", "Users.Manage"));

                // Role permissions
                options.AddPolicy("RequireRolesReadPermission", policy =>
                    policy.RequireClaim("permission", "Roles.Read"));
                options.AddPolicy("RequireRolesCreatePermission", policy =>
                    policy.RequireClaim("permission", "Roles.Create"));
                options.AddPolicy("RequireRolesUpdatePermission", policy =>
                    policy.RequireClaim("permission", "Roles.Update"));
                options.AddPolicy("RequireRolesDeletePermission", policy =>
                    policy.RequireClaim("permission", "Roles.Delete"));
                options.AddPolicy("RequireRolesManagePermission", policy =>
                    policy.RequireClaim("permission", "Roles.Manage"));

                // Permission permissions
                options.AddPolicy("RequirePermissionsReadPermission", policy =>
                    policy.RequireClaim("permission", "Permissions.Read"));
                options.AddPolicy("RequirePermissionsManagePermission", policy =>
                    policy.RequireClaim("permission", "Permissions.Manage"));

                // Dashboard permissions
                options.AddPolicy("RequireDashboardReadPermission", policy =>
                    policy.RequireClaim("permission", "Dashboard.Read"));
                options.AddPolicy("RequireDashboardManagePermission", policy =>
                    policy.RequireClaim("permission", "Dashboard.Manage"));

                // Reports permissions
                options.AddPolicy("RequireReportsReadPermission", policy =>
                    policy.RequireClaim("permission", "Reports.Read"));
                options.AddPolicy("RequireReportsCreatePermission", policy =>
                    policy.RequireClaim("permission", "Reports.Create"));
                options.AddPolicy("RequireReportsManagePermission", policy =>
                    policy.RequireClaim("permission", "Reports.Manage"));

                // Admin role requirement
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin", "SuperAdmin"));
                options.AddPolicy("RequireSuperAdminRole", policy =>
                    policy.RequireRole("SuperAdmin"));
            });

            return services;
        }
    }
} 