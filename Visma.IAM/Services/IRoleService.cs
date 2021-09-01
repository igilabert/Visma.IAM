using System.Collections.Generic;
using Visma.IAM.Models;

namespace Visma.IAM.Services
{
    /// <summary>
    /// Service in charge of adding roles and permissions 
    /// </summary>
    public interface IRoleService
    {                
        /// <summary>
        /// Added the given permissions to the desired role if exists.
        /// </summary>
        /// <param name="roleName">Role to add permissions</param>
        /// <param name="permissions">List of permissions to add</param>
        void AddPermissionsToRole(string roleName, List<Permission> permissions);        
        
        /// <summary>
        /// Gets a role using its name
        /// </summary>
        /// <param name="roleName">Role name to retrieve</param>
        IRole GetRole(string roleName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<IRole> GetRoles();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="role"></param>
        void RegisterRole(IRole role);
    }
}
