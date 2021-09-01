using System.Collections.Generic;
using Visma.IAM.Models;
using Xunit;

namespace Visma.IAM.Tests.Models
{
    public class EmployeeRoleTests
    {
        [Fact]
        public void CanBeCreated()
        {
            Assert.NotNull(new EmployeeRole());
        }

        [Fact]
        public void NameMustBeAssignedToEmployee_WhenCreated()
        {
            var employeeRole = new EmployeeRole();

            Assert.Equal("Employee", employeeRole.Name);
        }

        [Fact]
        public void ManagerRole_MustContainDefaultPermissions_WhenCreated()
        {
            var expectedPermissions = new List<Permission> { Permission.Read };
            
            var employeeRole = new EmployeeRole();

            Assert.Equal(expectedPermissions, employeeRole.Permissions);
        }
    }
}
