namespace BaseAuth.Domain.Constants
{
    public static class Permissions
    {
        // User permissions
        public static class Users
        {
            public const string Read = "Users.Read";
            public const string Create = "Users.Create";
            public const string Update = "Users.Update";
            public const string Delete = "Users.Delete";
            public const string Manage = "Users.Manage";
            public const string Export = "Users.Export";
            public const string Import = "Users.Import";
        }

        // Role permissions
        public static class Roles
        {
            public const string Read = "Roles.Read";
            public const string Create = "Roles.Create";
            public const string Update = "Roles.Update";
            public const string Delete = "Roles.Delete";
            public const string Manage = "Roles.Manage";
            public const string Assign = "Roles.Assign";
        }


        // Dashboard permissions
        public static class Dashboard
        {
            public const string Read = "Dashboard.Read";
            public const string Manage = "Dashboard.Manage";
            public const string Analytics = "Dashboard.Analytics";
        }

        // Reports permissions
        public static class Reports
        {
            public const string Read = "Reports.Read";
            public const string Create = "Reports.Create";
            public const string Update = "Reports.Update";
            public const string Delete = "Reports.Delete";
            public const string Export = "Reports.Export";
            public const string Import = "Reports.Import";
            public const string Manage = "Reports.Manage";
        }

        // Settings permissions
        public static class Settings
        {
            public const string Read = "Settings.Read";
            public const string Update = "Settings.Update";
            public const string Manage = "Settings.Manage";
            public const string System = "Settings.System";
        }

        // System permissions
        public static class System
        {
            public const string Read = "System.Read";
            public const string Manage = "System.Manage";
            public const string Admin = "System.Admin";
            public const string Maintenance = "System.Maintenance";
        }

        // Profile permissions
        public static class Profile
        {
            public const string Read = "Profile.Read";
            public const string Update = "Profile.Update";
            public const string Delete = "Profile.Delete";
        }

        // Analytics permissions
        public static class Analytics
        {
            public const string Read = "Analytics.Read";
            public const string Export = "Analytics.Export";
            public const string Manage = "Analytics.Manage";
        }

        // Data permissions
        public static class Data
        {
            public const string Read = "Data.Read";
            public const string Export = "Data.Export";
            public const string Import = "Data.Import";
            public const string Backup = "Data.Backup";
            public const string Restore = "Data.Restore";
        }

        // Audit permissions
        public static class Audit
        {
            public const string Read = "Audit.Read";
            public const string Export = "Audit.Export";
            public const string Manage = "Audit.Manage";
        }

        // Notification permissions
        public static class Notifications
        {
            public const string Read = "Notifications.Read";
            public const string Send = "Notifications.Send";
            public const string Manage = "Notifications.Manage";
        }

        // API permissions
        public static class Api
        {
            public const string Access = "Api.Access";
            public const string Manage = "Api.Manage";
            public const string RateLimit = "Api.RateLimit";
        }

        // Helper methods
        public static class Helper
        {
                    public static string[] GetAllPermissions()
        {
            return typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields())
                .Where(f => f.IsStatic && f.IsLiteral && !f.IsInitOnly)
                .Select(f => f.GetValue(null)?.ToString())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray()!;
        }

                    public static string[] GetPermissionsByResource(string resource)
        {
            var resourceType = typeof(Permissions).GetNestedType(resource);
            if (resourceType == null)
                return Array.Empty<string>();

            return resourceType.GetFields()
                .Where(f => f.IsStatic && f.IsLiteral && !f.IsInitOnly)
                .Select(f => f.GetValue(null)?.ToString())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray()!;
        }

            public static string[] GetResources()
            {
                return typeof(Permissions)
                    .GetNestedTypes()
                    .Where(t => t.IsClass && t.IsNestedPublic)
                    .Select(t => t.Name)
                    .ToArray();
            }
        }
    }
} 