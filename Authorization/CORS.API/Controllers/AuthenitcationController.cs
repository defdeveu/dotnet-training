﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CORS.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthenitcationController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenitcationController
        (
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor
        )
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _configuration = configuration;
                _httpContextAccessor = httpContextAccessor;
            }

        /// <summary>
        /// Authenticate to API (uses session cookies) 
        /// </summary>
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                _httpContextAccessor.HttpContext.Response.Cookies.Append("User", appUser.UserName);
                return Ok();
            }
            return NotFound();

            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        /// <summary>
        /// Add new user to API (uses session cookies) 
        /// </summary>
        // PUT api/values/5
        [HttpPut]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok();
            }
            return NotFound();
        }
    }
}
