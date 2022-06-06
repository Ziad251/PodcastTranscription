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
using System.Security.Principal;
using System.Linq;

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
