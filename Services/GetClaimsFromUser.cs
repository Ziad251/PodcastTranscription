using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace transcription_project.WebApp.Services
{
    public class GetClaimsFromUser : IGetClaimsProvider
    {

        public string Username { get; set; }
        public string Email { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        public GetClaimsFromUser(IHttpContextAccessor accessor)
        {
            Username = accessor.HttpContext?.User.Identity.Name;
            Email = accessor.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == "emails")?.Value;
        }
    }
}