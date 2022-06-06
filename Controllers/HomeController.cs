using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using transcription_project.WebApp.Models;
using transcription_project.WebApp.Services;
<<<<<<< HEAD
using System.Security.Principal;
using System.Linq;
=======
>>>>>>> cfca44a87104f08a8c3d7d53b067cc3192fea55e

namespace transcription_project.WebApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly IGetClaimsProvider _claims;
        private readonly IRepositoryService _repository;


        public HomeController(IGetClaimsProvider claims, IRepositoryService repository)
        {
            _claims = claims;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> Search()
        {
            var email = User.Claims.SingleOrDefault(x => x.Type == "emails")?.Value;

            if (_repository.FindUserByEmail(email) == null)
            {
                UserData newuser = await _repository.AddUser(email);
                await _repository.SaveAsync();
            }

            return View();
        }

        [Authorize]
        [HttpGet("results")]
        public IActionResult Results()
        {
            return View();
        }

<<<<<<< HEAD
=======
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] string username, string password)
        {
            // username check for duplicates.
            if (_repository.FindUserByUsername(username) != null)
            {
                TempData["Error"] = "Username must be unique. Try again.";
                return Redirect("/Signup/");
            }

            // password and username length check. If it checks out the newuser is added to database and signed in.
            if (username.Length >= 8 & password.Length >= 8)
            {
                await HttpContext.SignOutAsync();
                UserData newuser = await _repository.AddUser(username, password);
                await _repository.SaveAsync();
                await SignInUser(username, password, newuser);
                return Redirect("/Home/Index/");
            }

            TempData["Error"] = "Password and Username have to be at least 8 characters each.";
            return Redirect("/Signup/");
        }
        

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl))
            {
                returnUrl = "/Home/Index";
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            UserData user;
            try
            {
                user = await _repository.FindUserByUsername(username);
            }
            catch (Exception e)
            {
                TempData["Error"] = "Invalid Username or Password!";
                return View("login");
            }

            if (user == null)
            {
                TempData["Error"] = "Invalid Username or Password!";
                return View("login");
            }

            if (user.Password == password)
            {
                await HttpContext.SignOutAsync();
                await SignInUser(username, password, user);
                if (returnUrl is not null)
                {
                    return Redirect(returnUrl);
                }
            }

            TempData["Error"] = "Invalid Username or Password!";
            return View("login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/Home/Index");
        }

>>>>>>> cfca44a87104f08a8c3d7d53b067cc3192fea55e
        [Authorize]
        [HttpGet]
        public IActionResult UploadFiles()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllContainers()
        {
            UserData user = await _repository.FindUserByEmail(_claims.Email);
            try
            {
                string[] containers = await _repository.GetAllContainerNames(user);
                ViewData["Model"] = JsonSerializer.Serialize(containers);
            }
            catch (Exception e)
            {
                TempData["null"] = "unable to fetch containers";
            }

            return View();
        }


        //Users can only request transcription for listnames they have on their account field
        [Authorize]
        [HttpGet("Home/RequestTranscription/{listName}")]
        public IActionResult RequestTranscription([FromRoute] string listName)
        {
            ViewData["listname"] = listName;
            return View();
        }

        [Authorize]
        [HttpGet("Home/RequestCustomTranscription/{listName}")]
        public IActionResult RequestCustomTranscription([FromRoute] string listName)
        {
            ViewData["listname"] = listName;
            return View();
        }
        


    }
}
