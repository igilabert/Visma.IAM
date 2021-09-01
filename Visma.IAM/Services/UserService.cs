using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Visma.IAM.Models;

[assembly: InternalsVisibleTo("Visma.IAM.Tests")]

namespace Visma.IAM.Services
{
    public class UserService : IUserService
    {        
        internal List<User> _users = new();

        public void AddRoleToUser(string userMail, IRole role)
        {
            if (!UserExists(userMail)) throw new KeyNotFoundException("User does not exist");

            var userToModify = _users.FirstOrDefault(x => x.Email == userMail);

            if (userToModify.AssignedRoles.Any(x => x.Name == role.Name)) throw new ArgumentException($"User already is assigned to {role.Name} role");

            userToModify.AssignedRoles.Add(role);            
        }

        public List<Permission> GetUserPermissions(string userMail)
        {
            if (!UserExists(userMail)) throw new KeyNotFoundException("User does not exist");

            var user = _users.FirstOrDefault(x => x.Email == userMail);

            return GetPermissions(user.AssignedRoles);
        }

        public List<User> GetUsers() => _users;        

        public void RegisterUser(User user)
        {
            if (UserExists(user.Email)) throw new ArgumentException("User with that mail already exists.");

            _users.Add(user);                 
        }

        private bool UserExists(string email) => _users.Any(x => x.Email == email);   
        
        private static List<Permission> GetPermissions(List<IRole> roles)
        {
            List<Permission> permissions = new();
            
            foreach (var role in roles)
            {
                permissions = permissions.Union(role.Permissions).ToList();
            }

            return permissions;
        }
    }
}
