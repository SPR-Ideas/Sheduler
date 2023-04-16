﻿using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebApp_Entity.Models;

using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApp_Entity.Controllers
{
    
    public class HomeController : Controller
    {
        public static UserModel User { get; set; }
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult login( UserModel user) {

            if (user.verifyUser()) {
                User = user;
                string token = CreateToken();
                //Response.Headers.Add("Authorization", "Bearer "+ token);
                Response.Cookies.Append("auth_token", token);
                return RedirectToAction("dashboard");
            }
            return View();
        }

        [HttpPost]
        public string CreateToken() {
            List<Claim> claim = new List<Claim>() {
            new Claim(ClaimTypes.Name, User.username)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value
                ));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claim,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: cred
                ) ;
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    

        [HttpGet]
        public IActionResult register() {
        
            return View();
        }

        private UserModel getCurrentUser() {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            Console.WriteLine(identity);

            string authorizationHeader = HttpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                // Do something with the token
                Console.WriteLine(token);
            }
            if (identity != null) {
                var userClaims = identity.Claims;
                return new UserModel{ username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value};
            }
            return null;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult dashboard() {
            UserModel user = getCurrentUser();
            ViewBag.user = user;
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}