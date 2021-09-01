using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Visma.IAM.Controllers;
using Visma.IAM.Models;
using Visma.IAM.Services;
using Xunit;

namespace Visma.IAM.Tests.Controllers
{
    public class RolesControllerTests
    {
        private Mock<IRoleService> _roleServiceMock = new();
       
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void RegisterRole_MustReturnBadRequest_IfRoleNameIsNullOrWhitespace(string value)
        {
            var roleController = new RolesController(_roleServiceMock.Object);

            var result = roleController.RegisterRole(new Role (value, null));

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void RegisterRole_MustReturnBadRequest_IfRoleAlreadyExists()
        {
            _roleServiceMock.Setup(x => x.RegisterRole(It.IsAny<Role>())).Throws(new ArgumentException("Role already exists"));

            var roleController = new RolesController(_roleServiceMock.Object);

            var result = roleController.RegisterRole(new Role("value", null));

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Role already exists", (result.Result as BadRequestObjectResult).Value);
        }       

        [Fact]
        public void RegisterRole_MustReturnCreatedRole_IfRoleServiceCanRegisterTheRole()
        {
            var role = new Role("TestRole", new List<Permission> { Permission.Read, Permission.Create });

            var roleController = new RolesController(_roleServiceMock.Object);

            var result = roleController.RegisterRole(role);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(role, (result.Result as OkObjectResult).Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void AddPermissionsToRole_MustReturnBadRequest_IfRoleNameIsNullOrWhitespace(string roleName)
        {
            var roleController = new RolesController(_roleServiceMock.Object);

            var result = roleController.AddPermissions(roleName, new List<Permission> { Permission.Delete, Permission.Delete });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            _roleServiceMock.Verify(x => x.AddPermissionsToRole(It.IsAny<string>(), It.IsAny<List<Permission>>()), Times.Never);
        }

        [Fact]
        public void AddPermissions_MustReturnNotFound_IfRoleToAddPermissionsDoesNotExists()
        {
            var expectedPermissions = new List<Permission> { Permission.Delete, Permission.Update };
            _roleServiceMock.Setup(x => x.AddPermissionsToRole(It.IsAny<string>(), It.IsAny<List<Permission>>())).Throws(new KeyNotFoundException("Role does not exist"));

            var roleController = new RolesController(_roleServiceMock.Object);

            var result = roleController.AddPermissions("TestRole", expectedPermissions);

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result);
            _roleServiceMock.Verify(x => x.AddPermissionsToRole("TestRole", expectedPermissions), Times.Once);
        }

        [Fact]
        public void AddPermissions_MustReturnNotFound_IfRoleServiceThrowsArgumentException()
        {
            var expectedPermissions = new List<Permission> { Permission.Delete, Permission.Update };            
            _roleServiceMock.Setup(x => x.AddPermissionsToRole("TestRole", expectedPermissions)).Throws(new ArgumentException("Permision is not valid."));

            var roleController = new RolesController(_roleServiceMock.Object);

            var result = roleController.AddPermissions("TestRole", expectedPermissions);

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            _roleServiceMock.Verify(x => x.AddPermissionsToRole("TestRole", expectedPermissions), Times.Once);
        }

        [Fact]
        public void AddPermissions_MustReturnNoContent_IfRoleServiceCanAddPermissionsToRole()
        {
            var expectedPermissions = new List<Permission> { Permission.Delete, Permission.Update };                                             

            var roleController = new RolesController(_roleServiceMock.Object);            

            var result = roleController.AddPermissions("TestRole", expectedPermissions);

            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result);
            _roleServiceMock.Verify(x => x.AddPermissionsToRole("TestRole", expectedPermissions), Times.Once);
        }

        [Fact]
        public void GetRoles_MustReturnDefaultRoles_IfNoRolesHaveBeenCreated()
        {            
            var defaultRoles = new List<IRole> { new EmployeeRole(), new ManagerRole() };
            var rolesController = new RolesController(_roleServiceMock.Object);

            var result = rolesController.GetRoles();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);

            _roleServiceMock.Verify(x => x.GetRoles(), Times.Once);
        }

        [Fact]
        public void GetRoles_MustReturnRoles_WhenRolesHaveBeenCreated()
        {            
            var expectedRoles = new List<IRole> { new EmployeeRole(), new ManagerRole() , new Role("TestRole")};
            var rolesController = new RolesController(_roleServiceMock.Object);

            var result = rolesController.GetRoles();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);

            _roleServiceMock.Verify(x => x.GetRoles(), Times.Once);
        }
    }
}
