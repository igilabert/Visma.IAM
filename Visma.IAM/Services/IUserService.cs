using System.Collections.Generic;
using Visma.IAM.Models;

namespace Visma.IAM.Services
{
    /// <summary>
    /// Service in charge of managing users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers the given usser into the system.
        /// </summary>
        /// <param name="user">User to register</param>
        void RegisterUser(User user);

        /// <summary>
        /// Add role to a user by email
        /// </summary>                
        /// <param name="userMail">Desired user email</param>
        /// <param name="role">Role to add</param>
        void AddRoleToUser(string userMail, IRole role);
        
        /// <summary>
        /// Gets the list of users
        /// </summary>
        /// <returns>List of users in the system</returns>
        List<User> GetUsers();

        /// <summary>
        /// Gets the user permissions based on her roles
        /// </summary>
        /// <param name="userMail">Desired user email</param>
        /// <returns>A list of permissions for the given user based on her roles</returns>
        List<Permission> GetUserPermissions(string userMail);
    }
}
