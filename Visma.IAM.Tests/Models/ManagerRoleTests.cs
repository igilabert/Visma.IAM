using System.Collections.Generic;
using Visma.IAM.Models;
using Xunit;

namespace Visma.IAM.Tests.Models
{
    public class ManagerRoleTests
    {
        [Fact]
        public void CanBeCreated()
        {
            Assert.NotNull(new ManagerRole());
        }

        [Fact]
        public void NameMustBeAssignedToManager_WhenCreated()
        {
            var managerRole = new ManagerRole();

            Assert.Equal("Manager", managerRole.Name);
        }

        [Fact]
        public void ManagerRole_MustContainDefaultPermissions_WhenCreated()
        {
            var expectedPermissions = new List<Permission> { Permission.Create, Permission.Read, Permission.Update, Permission.Delete };
            
            var managerRole = new ManagerRole();

            Assert.Equal(expectedPermissions, managerRole.Permissions);
        }
    }
}
