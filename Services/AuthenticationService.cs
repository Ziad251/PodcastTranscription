// using System.Collections.Generic;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Http;
// using transcription_project.WebApp.Models;

// namespace transcription_project.WebApp.Services
// {
//     public class AuthenticationService
//     {
//         private async Task SignInUser(string username, string password, UserData newuser)
//         {
//             var claims = new List<Claim>();
//             claims.Add(new Claim("uid", newuser.id.ToString()));
//             claims.Add(new Claim("username", username));
//             claims.Add(new Claim("password", password));
//             var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//             var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//             await HttpContext.SignInAsync(claimsPrincipal);
//         }
//     }
// }