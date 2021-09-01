using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Visma.IAM.Dto;
using Visma.IAM.Models;
using Visma.IAM.Services;

namespace Visma.IAM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;        
        
        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_userService.GetUsers());
        }

        [HttpGet("{email}")]
        public ActionResult<IEnumerable<Permission>> GetUserPermissions(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("User email cannot be empty");

            List<Permission> permissions;
            try
            {
                permissions =_userService.GetUserPermissions(email);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok(permissions);
        }

        [HttpPost]
        public ActionResult<UserDto> RegisterUser(UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Email)) return BadRequest("User email cannot be empty");                                   

            try
            {
                _userService.RegisterUser(new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Department = userDto.Department,
                    Password = userDto.Department,
                    AssignedRoles = new List<IRole> { new EmployeeRole() }
                });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            // Notify manager! Maybe we can send an event to a notification service which will notify the manager? 
            // Otherwise we can obtain the manager/s for the department (via userService) and then send the notification from here

            // In the returned object we must not show the PWD
            return Ok(userDto);            
        }

        [HttpPut("{userMail}/{roleName}")]
        public ActionResult<User> AddRoleToUser(string userMail, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userMail)) return BadRequest("User email cannot be empty");

            if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name cannot be empty");

            var roleToAdd = _roleService.GetRole(roleName);

            if (roleToAdd == null) return NotFound("Role does not exist");

            try
            {
                _userService.AddRoleToUser(userMail, roleToAdd);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            

            return NoContent();            
        }
    }
}
