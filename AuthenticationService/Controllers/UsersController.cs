using System;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthenticationService.Models;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Services;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;

        public UsersController(ILogger logger, IUserService service, IWebHostEnvironment environment)
        {
            _logger = logger;
            _userService = service;
            _environment = environment;
        }

        // GET: api/Users/Authentication
        [HttpGet("Authentication")]
        public async Task<ActionResult<string>> Login()
        {
            _logger.Information("Triggered Login endpoint");
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return BadRequest("Missing Authorization header");
            }

            string authHeader = Request.Headers.SingleOrDefault(header => header.Key == "Authorization").Value;

            if (!authHeader.StartsWith("Basic "))
            {
                return BadRequest("Invalid auth type");
            }

            var (login, password) = Utils.GetLoginPasswordFromAuthHeader(authHeader);

            var (isSucceed, token) = await _userService.Login(login, password);

            if (isSucceed)
            {
                return Ok(token);
            }

            return BadRequest();
        }

        // GET: api/Users/Refresh
        [Authorize]
        [HttpGet("Refresh")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            string authHeader = Request.Headers.SingleOrDefault(header => header.Key == "Authorization").Value;

            if (!authHeader.StartsWith("Bearer "))
            {
                return BadRequest("Invalid auth type");
            }

            var (isSucceed, token) = await _userService.Refresh(User.Claims);

            if (isSucceed)
            {
                return Ok(token);
            }

            return BadRequest();
        }

        // GET: api/Users/Check
        [Authorize]
        [HttpGet("Check")]
        public async void CheckAuth() { }

        // GET: api/Users/Me
        [Authorize]
        [HttpGet("Me")]
        public async Task<ActionResult<string>> GetMe()
        {
            string authHeader = Utils.GetLoginFromClaims(User.Claims);

            return authHeader;
        }

        // DELETE: api/Users/Logout
        [Authorize]
        [HttpDelete("Logout")]
        public async Task<ActionResult<string>> Logout()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return BadRequest("Missing Authorization header");
            }

            string authHeader = Request.Headers.SingleOrDefault(header => header.Key == "Authorization").Value;

            if (!authHeader.StartsWith("Bearer "))
            {
                return BadRequest("Invalid auth type");
            }

            bool isSucceed = await _userService.CloseSession(User.Claims);

            if (isSucceed)
            {
                return Ok();
            }

            return BadRequest();
        }

        // GET: api/Users/5
        [Authorize(Policy = "CanViewAllUsers")]
        [HttpGet("{id}")]
        public async Task<ActionResult<int>> GetUser(int id)
        {
            var (isSucceed, user) = await _userService.GetUser(id);

            if (isSucceed)
            {
                return Ok(user);
            }

            return BadRequest();
        }

        // GET: api/Users/
        // GET: api/Users/all
        [Authorize(Policy = "CanViewAllUsers")]
        [HttpGet("All")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var (isSucceed, users) = await _userService.GetAllUsers();

            if (isSucceed)
            {
                return Ok(users);
            }

            return BadRequest();
        }

        //For authorization test
        [Authorize]
        [HttpGet("AuthorizeTest")]
        public async Task<ActionResult<string>> TestAuth()
        {
            string login = Utils.GetLoginFromClaims(User.Claims);
            string role = Utils.GetUserRoleFromClaims(User.Claims);
            return Ok($"Success!\n{login}, you are {role}");
        }

        // PUT: api/Users/Change/Me
        [Authorize]
        [HttpPut("Change/Me")]
        public async Task<ActionResult<int>> PutUser([FromBody]UserChangeModel user)
        {
            User newUser = new User();
            newUser.FromUserChangeModel(user);
            newUser.role = "user";

            var (isSucceed, token) = await _userService.ChangeUser(User.Claims, newUser);

            if (isSucceed)
            {
                return Ok(token);
            }

            return BadRequest();
        }

        // PUT: api/Users/Change/5
        [Authorize(Policy = "CanChangeAllUsers")]
        [HttpPut("Change/{id}")]
        public async Task<ActionResult<int>> PutUser(int id, [FromBody]UserChangeModel user)
        {
            User newUser = new User();
            newUser.FromUserChangeModel(user);

            var (isSucceed, token) = await _userService.ChangeUser(id, newUser);

            if (isSucceed)
            {
                return Ok(token);
            }

            return BadRequest();
        }

        // POST: api/Users/add
        [HttpPost("Add")]
        public async Task<ActionResult<int>> PostUser()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return BadRequest("Missing Authorization header");
            }

            string authHeader = Request.Headers.SingleOrDefault(header => header.Key == "Authorization").Value;

            if (!authHeader.StartsWith("Basic "))
            {
                return BadRequest("Invalid auth type");
            }

            var (login, password) = Utils.GetLoginPasswordFromAuthHeader(authHeader);

            var (isSucceed, token) = await _userService.Register(login, password);

            if (isSucceed)
            {
                return Ok(token);
            }

            return BadRequest();
        }

        // DELETE: api/Users/Delete/5
        [Authorize(Policy = "CanChangeAllUsers")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<int>> DeleteUser(int id)
        {
            bool isSucceed = await _userService.DeleteUser(id);

            if (isSucceed)
            {
                return Ok();
            }

            return BadRequest();
        }

        // DELETE: api/Users/Delete/Me
        [Authorize]
        [HttpDelete("Delete/Me")]
        public async Task<ActionResult<int>> DeleteCurrentUser()
        { 
            bool isSucceed = await _userService.DeleteUser(User.Claims);

            if (isSucceed)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
 