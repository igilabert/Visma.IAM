using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Visma.IAM.Models;
using Visma.IAM.Services;

namespace Visma.IAM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetRoles()
        {
            return Ok(_roleService.GetRoles());
        }

        [HttpPost]
        public ActionResult<Role> RegisterRole(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.Name)) return BadRequest("Role name cannot be empty");

            try
            {
                _roleService.RegisterRole(role);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            

            return Ok(role);
        }

        [HttpPut("{roleName}")]
        public ActionResult AddPermissions(string roleName, List<Permission> permissions)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name cannot be empty");            

            try
            {
                _roleService.AddPermissionsToRole(roleName, permissions);
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
