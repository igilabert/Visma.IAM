using System;
using System.Collections.Generic;
using System.Linq;
using Visma.IAM.Models;
using Visma.IAM.Services;
using Xunit;

namespace Visma.IAM.Tests.Services
{
    public class RoleServiceTests
    {
        private const string ExistingRole = "ExistingRole";
        private readonly Role _existingRole = new(ExistingRole, null);

        [Fact]
        public void CanBeCreated()
        {
            Assert.NotNull(new RoleService());
        }

        [Fact]
        public void RegisterRole_MustThrowArgumentException_IfARoleWithSameNameAlreadyExists()
        {
            RoleService roleService = new();
            roleService._roles.Add(_existingRole);
            
            Assert.Throws<ArgumentException>(() => roleService.RegisterRole(_existingRole));
        }

        [Fact]
        public void RegisterRole_MustThrowArgumentException_IfAnyPermissionIsInvalid()
        {
            RoleService roleService = new();
            var invalidRole = new Role("Something", new List<Permission> { Permission.Create, (Permission)7 });

            Assert.Throws<ArgumentException>(() => roleService.RegisterRole(invalidRole));
        }

        [Fact]
        public void RegisterRole_MustRegisterTheRole_IfNoRoleExistsWithSameName()
        {
            RoleService roleService = new();

            roleService.RegisterRole(new Role("TestRole", new List<Permission>()));

            Assert.True(roleService._roles.Exists(x => x.Name == "TestRole"));
        }

        [Fact]
        public void AddPermissionsToRole_MustThrowKeyNotFoundException_IfRoleDoesNotExists()
        {
            RoleService roleService = new();

            Assert.Throws<KeyNotFoundException>(() => roleService.AddPermissionsToRole("TestRole", new List<Permission> { Permission.Create, Permission.Delete }));
        }


        [Fact]
        public void AddPermissionsToRole_MustThrowArgumentException_IfPermissionsAreNotValid()
        {
            RoleService roleService = new();
            
            roleService._roles.Add(new Role("TestRole", new List<Permission> { Permission.Create }));

            Assert.Throws<ArgumentException>(() => roleService.AddPermissionsToRole("TestRole", new List<Permission> { Permission.Create, Permission.Delete, (Permission)7 }));
        }

        [Fact]
        public void AddPermissionsToRole_MustUpdateRolePermissions_IfRoleExists()
        {
            RoleService roleService = new();

            var expectedPermissions = new List<Permission> { Permission.Delete, Permission.Read, Permission.Update };

            roleService._roles.Add(new Role ("TestRole", new List<Permission> { Permission.Create }));

            roleService.AddPermissionsToRole("TestRole", expectedPermissions);

            var updatedRole = roleService._roles.First(x => x.Name == "TestRole");

            Assert.Equal(expectedPermissions, updatedRole.Permissions);
        }

        [Fact]
        public void AddPermissionsToRole_MustNotAddDuplicatePermissions_IfRoleExists()
        {
            RoleService roleService = new();

            var expectedPermissions = new List<Permission> { Permission.Delete, Permission.Read, Permission.Update, Permission.Delete, Permission.Read, Permission.Update  };

            roleService._roles.Add(new Role ("TestRole", new List<Permission> { Permission.Create }));

            roleService.AddPermissionsToRole("TestRole", expectedPermissions);

            var updatedRole = roleService._roles.First(x => x.Name == "TestRole");

            Assert.Equal(3, updatedRole.Permissions.Count);
            Assert.Equal(expectedPermissions.Distinct().ToList(), updatedRole.Permissions);
        }

        [Fact]
        public void GetRole_MustReturnNull_IfRoleDoesNotExist()
        {
            RoleService roleService = new();

            Assert.Null(roleService.GetRole("NonExistingRole"));
        }

        [Fact]
        public void GetRole_MustReturnRequestedRole_IfRoleExists()
        {
            RoleService roleService = new();

            var expectedRole = new Role("ExistingRole", new List<Permission> { Permission.Read, Permission.Create });

            roleService._roles.Add(expectedRole);

            var role = roleService.GetRole("ExistingRole");

            Assert.NotNull(role);
            Assert.Equal(expectedRole, role);
        }

        [Fact]
        public void GetRoles_MustReturnDefaultRoles_InNoRoleHasBeenCreated()
        {            
            RoleService roleService = new();

            var roles = roleService.GetRoles();

            Assert.Equal(2, roles.Count);
            Assert.NotNull(roles.FirstOrDefault(x => x.Name == "Employee"));
            Assert.NotNull(roles.FirstOrDefault(x => x.Name == "Manager"));
        }
    }
}
