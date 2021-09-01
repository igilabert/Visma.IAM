using System;
using System.Collections.Generic;
using System.Linq;
using Visma.IAM.Models;
using Visma.IAM.Services;
using Xunit;

namespace Visma.IAM.Tests.Services
{
    public class UserServiceTests
    {
        private const string ExistingMail = "existingMail";

        [Fact]
        public void CanBeCreated()
        {
            Assert.NotNull(new UserService());
        }

        [Fact]
        public void AddUser_MustThrowArgumentException_IfUserWithSameMailExists()
        {
            UserService userService = new();

            userService._users.Add(new User { Email = ExistingMail });

            Assert.Throws<ArgumentException>(() =>  userService.RegisterUser(new User { Email = ExistingMail }));            
        }

        [Fact]
        public void AddUser_MustReturnUser_IfUserHasBeenAdded()
        {
            UserService userService = new();
            var user = new User { Email = "newEmail" };

            userService._users.Add(new User { Email = ExistingMail });
            userService.RegisterUser(user);
            
            Assert.Contains(user, userService._users);
        }

        [Fact]
        public void AddRoleToUser_MustThrowKeyNotFoundException_IfUserDoesNotExist()
        {
            UserService userService = new();            

            Assert.Throws<KeyNotFoundException>(() => userService.AddRoleToUser("FakeMail", new Role("RoleName", null)));
        }

        [Fact]
        public void AddRoleToUser_MustThrowArgumentException_WhenUserAlreadyContainsTheRoleToAdd()
        {
            var expectedRole = new Role ("RoleName", new List<Permission> { Permission.Create });
            var existingUser = new User { Email = "UserMail" };
            existingUser.AssignedRoles.Add(new Role ("RoleName", null));

            UserService userService = new();

            userService._users.Add(existingUser);

            Assert.Throws<ArgumentException>(() => userService.AddRoleToUser("UserMail", expectedRole));
        }

        [Fact]
        public void AddRoleToUser_MustAddGivenRole_WhenUserExistsAndDoesNotContainTheRole()
        {
            var expectedRole = new Role ("TestRole", new List<Permission> { Permission.Create });
            UserService userService = new();

            userService._users.Add(new User { Email = "UserMail" });

            userService.AddRoleToUser("UserMail", expectedRole);

            var user = userService._users.FirstOrDefault(x => x.Email == "UserMail");
            var addedRole = user.AssignedRoles.Single();

            Assert.NotNull(addedRole);
            Assert.Equal(expectedRole, addedRole);            
        }

        [Fact]
        public void GetUsers_MustReturnEmptyList_IfNoUsersHaveBeenCreated()
        {
            UserService userService = new();

            Assert.Empty(userService.GetUsers());
        }

        [Fact]
        public void GetUsers_MustReturnAllUsers_WhenUsersHaveBeenCreated()
        {
            UserService userService = new();
            userService.RegisterUser(new User { Email = "Email1" });
            userService.RegisterUser(new User { Email = "Email2" });

            Assert.Equal(2, userService.GetUsers().Count);
        }

        [Fact]
        public void GetUserPermissions_MustThrowKeyNotFoundException_IfUserDoesNotExist()
        {
            UserService userService = new();

            Assert.Throws<KeyNotFoundException>(() => userService.GetUserPermissions("FakeMail"));            
        }

        [Fact]
        public void GetUserPermissions_ReturnsSimpleRolePermissions()
        {
            UserService userService = new();
            var user = new User { Email = "Email1", AssignedRoles = new List<IRole>() { new EmployeeRole() } };
            userService.RegisterUser(user);

            var permissions = userService.GetUserPermissions("Email1");

            Assert.Contains(Permission.Read, permissions);
            Assert.DoesNotContain(Permission.Delete, permissions);
            Assert.DoesNotContain(Permission.Create, permissions);
            Assert.DoesNotContain(Permission.Update, permissions);            
        }

        [Fact]
        public void GetUserPermissions_ReturnsUnionOfRolePermissions()
        {
            UserService userService = new();
            var user = new User { Email = "Email1", AssignedRoles = new List<IRole>() { new EmployeeRole(), new Role("TestRole", new List<Permission>() { Permission.Create, Permission.Update }) } };
            userService.RegisterUser(user);

            var permissions = userService.GetUserPermissions("Email1");

            Assert.Contains(Permission.Read, permissions);            
            Assert.Contains(Permission.Create, permissions);
            Assert.Contains(Permission.Update, permissions);
            Assert.DoesNotContain(Permission.Delete, permissions);
        }
    }
}
