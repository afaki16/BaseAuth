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
        }

        // Role permissions
        public static class Roles
        {
            public const string Read = "Roles.Read";
            public const string Create = "Roles.Create";
            public const string Update = "Roles.Update";
            public const string Delete = "Roles.Delete";
        }

        // Permission permissions
        public static class Permission
        {
            public const string Read = "Permissions.Read";
            public const string Create = "Permissions.Create";
            public const string Update = "Permissions.Update";
            public const string Delete = "Permissions.Delete";
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