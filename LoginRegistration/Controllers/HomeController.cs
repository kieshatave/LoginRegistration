using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using LoginRegistration.Models;

namespace LoginRegistration.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Invalid entry. Try again.");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("userId", user.UserId);
                return View("Success");
            }
            else
            {
              return View("Index");  
            }
            
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if(userId == null)
            {
                return View("Index");
            }
            return View("Success");
        }
        [HttpGet("login")]
        public ViewResult Login()
        {
            return View("Login");
        }
        [HttpPost("login")]
        public IActionResult Login(Login logIn)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == logIn.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid entry. Try again.");
                    return View("Login");
                }
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(logIn, userInDb.Password, logIn.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid entry. Try again.");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return View("Success");
            }
            else
            {
                return View("Index");
            }
            
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
