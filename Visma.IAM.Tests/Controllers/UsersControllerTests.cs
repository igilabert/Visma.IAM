using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Visma.IAM.Controllers;
using Visma.IAM.Dto;
using Visma.IAM.Models;
using Visma.IAM.Services;
using Xunit;

namespace Visma.IAM.Tests.Controllers
{
    public class UsersControllerTests
    {
        private Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IRoleService> _roleServiceMock = new();

        private const string FakeRole = "FakeRole";
        private const string Email = "Email";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void RegisterUser_MustReturnBadRequest_IfUserEmailIsNullOrWhitespace(string value)
        {            
            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.RegisterUser(new UserDto { Email = value });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);                 
        }       

        [Fact]
        public void RegisterUser_MustReturnBadRequest_IfUserServiceThrowsAnArgumentException()
        {
            _userServiceMock.Setup(x => x.RegisterUser(It.IsAny<User>())).Throws(new ArgumentException("Email already exists"));

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.RegisterUser(new UserDto { Email = "value" });

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Email already exists", (result.Result as BadRequestObjectResult).Value);
        }

        [Fact]
        public void RegisterUser_MustReturnCreatedUser_IfUserServiceCanRegisterTheUser()
        {
            var user = new UserDto { Email = "value" };

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.RegisterUser(user);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, (result.Result as OkObjectResult).Value);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void AddRoleToUser_MustReturnBadRequest_IfUserEmailIsNullOrWhitespace(string userMail)
        {
            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.AddRoleToUser(userMail, "RoleName");

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void AddRoleToUser_MustReturnBadRequest_IfRoleNameIsNullOrWhitespace(string roleName)
        {
            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.AddRoleToUser("UserMail", roleName);

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddRoleToUser_MustReturnNotFound_IfRoleToAddDoesNotExist()
        {
            _roleServiceMock.Setup(x => x.GetRole(FakeRole)).Returns(null as Role);


            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.AddRoleToUser("UserMail", FakeRole);

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);

            _roleServiceMock.Verify(x => x.GetRole(FakeRole), Times.Once);
        }

        [Fact]
        public void AddRoleToUser_MustReturnNotFound_IfUserDoesNotExist()        
        {            
            _roleServiceMock.Setup(x => x.GetRole(FakeRole)).Returns(new Role("TestRole", null));
            _userServiceMock.Setup(x => x.AddRoleToUser("UserMail", It.IsAny<Role>())).Throws(new KeyNotFoundException("User does not exist"));

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.AddRoleToUser("UserMail", FakeRole);

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);

            _roleServiceMock.VerifyAll();
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public void AddRoleToUser_MustReturnBadRequest_IfUserAlreadyContainsTheRoleToAdd()
        {
            _roleServiceMock.Setup(x => x.GetRole(FakeRole)).Returns(new Role("TestRole", null));
            _userServiceMock.Setup(x => x.AddRoleToUser("UserMail", It.IsAny<Role>())).Throws(new ArgumentException("User already is assigned to the role"));

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.AddRoleToUser("UserMail", FakeRole);

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

            _roleServiceMock.VerifyAll();
            _userServiceMock.VerifyAll();
        }

        [Fact]
        public void AddRoleToUser_MustReturnNoContent_IfRoleHasBeenAsignedToTheUser()
        {
            _roleServiceMock.Setup(x => x.GetRole(FakeRole)).Returns(new Role("TestRole", null));            

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.AddRoleToUser("UserMail", FakeRole);

            Assert.NotNull(result);
            Assert.IsType<NoContentResult>(result.Result);

            _roleServiceMock.VerifyAll();
            _userServiceMock.Verify(x => x.AddRoleToUser("UserMail", It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public void GetUsers_MustReturnEmtpyList_IfNoUsersHaveBeenCreated()
        {
            _userServiceMock.Setup(x => x.GetUsers()).Returns(new List<User>());

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.GetUsers();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            
            _userServiceMock.Verify(x => x.GetUsers(), Times.Once);
        }

        [Fact]
        public void GetUsers_MustReturnUsers_IfUsersHaveBeenCreated()
        {
            var expectedUsers = new List<User> { new User { Email = "Email1" }, new User { Email = "Email2" } };
            _userServiceMock.Setup(x => x.GetUsers()).Returns(expectedUsers);

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.GetUsers();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedUsers, ((OkObjectResult)result.Result).Value);

            _userServiceMock.Verify(x => x.GetUsers(), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void GetUserPermissions_MustReturnBadRequest_IfUserEmailIsNullOrWhitespace(string email)
        {
            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.GetUserPermissions(email);

            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result.Result);

            _userServiceMock.Verify(x => x.GetUserPermissions(email), Times.Never);
        }

        [Fact]
        public void GetUserPermissions_MustReturnNotFound_IfTheUserDoesNotExist()
        {
            var expectedPermissions = new List<Permission> { Permission.Create, Permission.Read, Permission.Update };
            
            _userServiceMock.Setup(x => x.GetUserPermissions(Email)).Throws(new KeyNotFoundException());

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.GetUserPermissions(Email);

            Assert.NotNull(result);
            Assert.IsType<NotFoundObjectResult>(result.Result);

            _userServiceMock.Verify(x => x.GetUserPermissions(Email), Times.Once);
        }

        [Fact]
        public void GetUserPermissions_MustReturnPermissions()
        {
            var expectedPermissions = new List<Permission> { Permission.Create, Permission.Read, Permission.Update };

            _userServiceMock.Setup(x => x.GetUserPermissions(Email)).Returns(expectedPermissions);

            var userController = new UsersController(_userServiceMock.Object, _roleServiceMock.Object);

            var result = userController.GetUserPermissions(Email);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedPermissions, ((OkObjectResult)result.Result).Value);

            _userServiceMock.Verify(x => x.GetUserPermissions(Email), Times.Once);
        }
    }
}
