using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileService.Validation;
using FileService.Model;
using FileService.Requests;
using FileService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FileService.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly TokenService tokenService;

        public AuthenticationController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            TokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var identityUser = new User()
            {
                UserName = request.Email,
                Email = request.Email
            };
            var result = await userManager.CreateAsync(identityUser, request.Password);
            
            if (!result.Succeeded) return BadRequest(new {Message = result.Errors.First()});
            
            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Email);
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            
            if (!result.Succeeded) 
                if (!result.Succeeded) return BadRequest(new {Message = "Login failed"});
            
            var tokenString = tokenService.BuildToken(user);
            return Ok(new {token = tokenString});
        }
    }
}