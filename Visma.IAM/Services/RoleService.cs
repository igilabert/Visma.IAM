using System;
using System.Collections.Generic;
using System.Linq;
using Visma.IAM.Models;

namespace Visma.IAM.Services
{
    public class RoleService : IRoleService
    {
        internal List<IRole> _roles = new() { new EmployeeRole(), new ManagerRole() };

        public void AddPermissionsToRole(string roleName, List<Permission> permissions)
        {
            if (!RoleExists(roleName)) throw new KeyNotFoundException($"{roleName} not exists. Create it before adding permissions");

            if (InvalidPermissions(permissions)) throw new ArgumentException("Permisions are not valid.");

            _roles.Where(x => x.Name == roleName).First().Permissions = permissions.Distinct().ToList(); // modifying a ref object :( // avoid duplicates
        }

        public IRole GetRole(string roleName) => _roles.FirstOrDefault(r => r.Name == roleName);

        public List<IRole> GetRoles() => _roles;

        public void RegisterRole(IRole role)
        {
            if (RoleExists(role.Name)) throw new ArgumentException($"{role.Name} role already exists.");

            if (InvalidPermissions(role.Permissions)) throw new ArgumentException("Permisions are not valid.");

            _roles.Add(role);
        }

        private static bool InvalidPermissions(List<Permission> permissions) => permissions.Where(x => !PermissionsHelper.IsValidPermission(x.ToString())).Any();        

        internal bool RoleExists(string roleName) => _roles.Any(r => r.Name == roleName);        
    }
}
