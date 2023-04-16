using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebApp_Entity.Models;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;

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
            new Claim(ClaimTypes.Name, User.username),
            new Claim(ClaimTypes.Sid,User.id),
            new Claim(ClaimTypes.MobilePhone,User.phone)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value
                ));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claim,
                expires: DateTime.Now.AddHours(2),
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
                return new UserModel {
                    username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
                    id =(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Sid)?.Value),
                    phone = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.MobilePhone)?.Value
                };
            }
            return null;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult dashboard() {
            UserModel user = getCurrentUser();
            ViewBag.user = user;
            MeetingModel meetings = new MeetingModel();
            meetings.getMeetingList(  Convert.ToInt32( user.id));
            ViewBag.meetings = meetings.MeetingList;

            return View();
        }

        [HttpGet]
        public IActionResult ConfirmAppointment(int id) {
            MeetingModel meet = new MeetingModel();
            meet.confirmMeeting(id);

            return RedirectToAction("dashboard");
        }

        public IActionResult DeleteAppointment(int id)
        {
            MeetingModel meet = new MeetingModel();
            meet.deleteMeeting(id);
            return RedirectToAction("dashboard");
        }

        public IActionResult MakeAppointment(Meet meet) { 
            MeetingModel mt = new MeetingModel();
            mt.makeAppointment(meet);
            return RedirectToAction("dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}