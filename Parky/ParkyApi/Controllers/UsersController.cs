﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkyApi.Models;
using ParkyApi.Repository.IRepository;

namespace ParkyApi.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/users")]
    //[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User model)
        {
            var user = _userRepo.Authenticate(model.UserName, model.Password);
            if(user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] User model)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            if(!ifUserNameUnique)
            {
                return BadRequest(new { message = "Username already exists!" });
            }
            var user = _userRepo.Register(model.UserName, model.Password);
            if(user == null)
            {
                return BadRequest(new { message = "Error while registering!" });
            }
            return Ok();
        }
    }
}