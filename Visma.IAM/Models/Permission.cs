using System;

namespace Visma.IAM.Models
{
    public enum Permission
    {
        Create,
        Read,
        Update,
        Delete
    }

    public static class PermissionsHelper
    {
        public static bool IsValidPermission(string permission) => Enum.TryParse(permission, out Permission _) && Enum.IsDefined(typeof(Permission), permission);        
    }
}
