using System;

namespace BaseAuth.Domain.Enums
{
    [Flags]
    public enum PermissionType
    {
        None = 0,
        Create = 1 << 0,        // 1
        Read = 1 << 1,          // 2
        Update = 1 << 2,        // 4
        Delete = 1 << 3,        // 8
        Manage = 1 << 4,        // 16
        Export = 1 << 5,        // 32
        Import = 1 << 6,        // 64
        Approve = 1 << 7,       // 128
        
        // Önceden tanımlanmış kombinasyonlar
        ReadWrite = Read | Create | Update,           // 2 | 1 | 4 = 7
        FullAccess = Create | Read | Update | Delete | Manage, // 31
        AdminAccess = FullAccess | Export | Import | Approve   // 255
    }
} 