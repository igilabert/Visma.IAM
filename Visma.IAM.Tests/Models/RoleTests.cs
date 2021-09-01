using System.Collections.Generic;
using Visma.IAM.Models;
using Xunit;

namespace Visma.IAM.Tests.Models
{
    public class RoleTests
    {
        private readonly List<Permission> _defaultEmployeePermissions = new() { Permission.Read };

        [Fact]
        public void CanBeCreated()
        {
            Assert.NotNull(new Role("TestRole", new List<Permission>()));
        }

        [Fact]
        public void NameIsAsigned_WhenCreated()
        {
            var role = new Role("TestRole", new List<Permission>());

            Assert.Equal("TestRole", role.Name);
        }

        [Fact]        
        public void DefaultEmployeePermissions_AreSet_WhenCreatedWithNullPermissions()
        {
            var role = new Role("TestRole", null);

            Assert.Equal(_defaultEmployeePermissions, role.Permissions);
        }

        [Fact]
        public void DefaultEmployeePermissions_AreSet_WhenCreatedWithEmptyPermissions()
        {
            var role = new Role("TestRole", new List<Permission>());

            Assert.Equal(_defaultEmployeePermissions, role.Permissions);
        }

        [Fact]
        public void GivenPermissions_AreSet_WhenCreatedWithPermissions()
        {
            var expectedPermissions = new List<Permission> { Permission.Create, Permission.Read };
            
            var role = new Role("TestRole", expectedPermissions);

            Assert.Equal(expectedPermissions, role.Permissions);
        }
    }
}
