using IdentityFromScratch.Areas.Identity.Data;
using IdentityFromScratch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace IdentityFromScratch.Controllers
{
    [Authorize(Policy = "RequireUserManager")]
    public class UsersController : Controller
    {

        // Dependency Injection

        private readonly UserManager<IdentityFromScratchUser> _usermanager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly SignInManager<IdentityFromScratchUser> _signinmanager;
        public UsersController(UserManager<IdentityFromScratchUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityFromScratchUser> signinManager)
        {
            _usermanager = userManager;
            _rolemanager = roleManager;
            _signinmanager = signinManager;
        }

        // GET: UsersController
        public async Task<ActionResult> Index()
        {
            
            var users = _usermanager.Users.ToList();
            Dictionary<string, IList<string>> roles = new Dictionary<string, IList<string>>();
            Dictionary<string, IList<Claim>> claims = new Dictionary<string, IList<Claim>>();
            foreach (var user in users)
            {
                roles.Add(user.Id, await _usermanager.GetRolesAsync(user));
                claims.Add(user.Id, await _usermanager.GetClaimsAsync(user));
            }
            ViewBag.roles = roles;
            ViewBag.claims = claims;

            return View(users);
        }

 
        public async Task<ActionResult> Roles(string id)
        {
            var user = _usermanager.Users.Where(s => s.Id == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound($"user {id} was not found");
            }

            var roles = _rolemanager.Roles.ToList();
            List<string> myRoles = (await _usermanager.GetRolesAsync(user)).ToList();

            ViewBag.myroles = myRoles;
            ViewBag.user = user;

            return View(roles);

        }

        public async Task<IActionResult> addrole(string role, string user)
        {
            var u = await _usermanager.FindByIdAsync(user);
            if (u == null) return NotFound($"User {user} not found");
            var r = await _rolemanager.FindByIdAsync(role);
            if (r == null) return NotFound($"Role {role} not found");
            await _usermanager.AddToRoleAsync(u, r.Name);
            await _signinmanager.RefreshSignInAsync(u);                         // refresh the user roles by recreating the cookie
            return RedirectToAction("Roles", new { Id = u.Id });
        }

        public async Task<IActionResult> removerole(string role, string user)
        {
            var u = await _usermanager.FindByIdAsync(user);
            if (u == null) return NotFound($"User {user} not found");
            var r = await _rolemanager.FindByIdAsync(role);
            if (r == null) return NotFound($"Role {role} not found");
            await _usermanager.RemoveFromRoleAsync(u, r.Name);
            await _signinmanager.RefreshSignInAsync(u);                       // refresh the user roles by recreating the cookie
            return RedirectToAction("Roles", new { Id = u.Id });
        }

        public async Task<IActionResult> addclaim(string claimtype, string value, string user)
        {

            // this method is functionally identical to the createclaim BUT returns to the index page
            // this method is invoked from the instant claim anchor
            // this method returns to the index page

            var u = await _usermanager.FindByIdAsync(user);
            if (u == null) return NotFound("User not found");
            var c = new System.Security.Claims.Claim(claimtype, value);
            var r = await _usermanager.AddClaimAsync(u, c);
            if (r.Succeeded)
            {
                return RedirectToAction("index");
            }
            else
            {
                foreach (var e in r.Errors)
                {
                    Console.WriteLine(e);
                }
            }
            await _signinmanager.RefreshSignInAsync(u);                       // refresh the user claims by recreating the cookie
            return RedirectToAction("index");

        }

        public async Task<IActionResult> IClaims(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound("User not Found");


            return View(user);
        }

        public async Task<IActionResult> ManageClaims(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound("User not Found");
            var claims = await _usermanager.GetClaimsAsync(user);

            ViewBag.user = user;

            return View(claims);
        }

        public async Task<IActionResult> removeclaim(string claimtype, string value, string user)
        {
            var u = await _usermanager.FindByIdAsync(user);
            if (u == null) return NotFound($"User {user} not found");
            var c = new System.Security.Claims.Claim(claimtype, value);

            var r = await _usermanager.RemoveClaimAsync(u, c);
            if (r.Succeeded)
            {
                return RedirectToAction("ManageClaims", new { Id = u.Id });
            }
            else
            {
                foreach (var e in r.Errors)
                {
                    Console.WriteLine(e);
                }
            }
            await _signinmanager.RefreshSignInAsync(u);                       // refresh the user claims by recreating the cookie
            return RedirectToAction("ManageClaims", new { Id = u.Id });

        }

        public async Task<IActionResult> createclaim(string claimtype, string value, string user)
        {

            // this method is functionally identical to the addclaim BUT returns to the ManageClaim page
            // this method is invoked from the Manageclaim anchor
            // this method returns to the ManageClaim page

            var u = await _usermanager.FindByIdAsync(user);
            if (u == null) return NotFound($"User {user} not found");
            var c = new System.Security.Claims.Claim(claimtype, value);
           

            var r = await _usermanager.AddClaimAsync(u, c);
            if (r.Succeeded)
            {
                
                return RedirectToAction("ManageClaims", new { Id = u.Id });
            }
            else
            {
                foreach (var e in r.Errors)
                {
                    Console.WriteLine(e);
                }
            }
            await _signinmanager.RefreshSignInAsync(u);                       // refresh the user claims by recreating the cookie

            return RedirectToAction("ManageClaims", new { Id = u.Id });

        }

        public async Task<IActionResult> ConfirmEmail(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound($"User {id} not found");
            var token = await _usermanager.GenerateEmailConfirmationTokenAsync(user);
            var r = await _usermanager.ConfirmEmailAsync(user, token);
            if (r.Succeeded)
            {
                return RedirectToAction("index");
            }
            else
            {
               
                StringBuilder sb = new StringBuilder();
                foreach(var e in r.Errors)
                {
                    sb.Append($"{e.Code}:{e.Description} <br/>");
                }
                return View("Error", new ErrorViewModel() {RequestId="Unknown",Message=sb.ToString()});
            }
        }

       
        public async Task<IActionResult> ResetPassword(string id,string newPassword)
        {
            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound($"User {id} not found");
            var token = await _usermanager.GeneratePasswordResetTokenAsync(user);
            var r = await _usermanager.ResetPasswordAsync(user,token,newPassword);
            if (r.Succeeded)
            {
                return RedirectToAction("index");
            }
            else
            {

                StringBuilder sb = new StringBuilder();
                foreach (var e in r.Errors)
                {
                    sb.Append($"{e.Code}:{e.Description} <br/>");
                }
                return View("Error", new ErrorViewModel() { RequestId = "Unknown", Message = sb.ToString() });
            }
        }

        public async Task<IActionResult> RemoveUser(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            if (user == null) return NotFound($"User {id} not found");
          
            var r = await _usermanager.DeleteAsync(user);
            if (r.Succeeded)
            {
                if (user.UserName == User?.Identity?.Name)
                {
                    await _signinmanager.SignOutAsync();
                }
                return RedirectToAction("index");
            }
            else
            {

                StringBuilder sb = new StringBuilder();
                foreach (var e in r.Errors)
                {
                    sb.Append($"{e.Code}:{e.Description} <br/>");
                }
                return View("Error", new ErrorViewModel() { RequestId = "Unknown", Message = sb.ToString() });
            }
        }


    }
}
