using IdentityFromScratch.Areas.Identity.Data;
using IdentityFromScratch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityFromScratch.Controllers
{
    [Authorize(Policy = "RequireUserManager")]
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityFromScratchUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityFromScratchUser> _signinmanager;

        public RolesController(UserManager<IdentityFromScratchUser> um, RoleManager<IdentityRole> rm, SignInManager<IdentityFromScratchUser> signinManager)
        {
            _userManager = um;
            _roleManager = rm;
            _signinmanager = signinManager;

        }

        // GET: RolesController
        public ActionResult Index()
        {
            var roles = _roleManager.Roles.Select( r=> new RoleViewModel() {Id = r.Id, Name = r.Name }    );
            return View(roles);
        }

        // GET: RolesController/Details/5
        public ActionResult Details(string id)
        {
            var role = _roleManager.Roles.Where(s => s.Id == id).Select(s => new RoleViewModel() { Id = s.Id, Name = s.Name }).FirstOrDefault();
            if (role == null)
            {
                return NotFound($"Role {id} was not found");
            }
            return View(role);
        }

        // GET: RolesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel collection)
        {
            try
            {
                var r = await _roleManager.CreateAsync(new IdentityRole(collection.Name));
                if (r.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View("Error", new ErrorViewModel() { RequestId="Unknown error while creating role" });
                }
            }
            catch
            {
                return View("Error", new ErrorViewModel() { RequestId = "Unknown exception while creating role" });
            }
        }

        // GET: RolesController/Edit/5
        public ActionResult Edit(string id)
        {
            var role = _roleManager.Roles.Where(s => s.Id == id).Select(s => new RoleViewModel() { Id = s.Id, Name = s.Name }).FirstOrDefault();
            if (role == null)
            {
                return NotFound($"Role {id} was not found");
            }
            return View(role);
        }

        // POST: RolesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, RoleViewModel collection)
        {
            try
            {
                IdentityRole role = await _roleManager.FindByIdAsync(id);
                if (role == null) return NotFound($"Role {id} was not found");
                role.Name = collection.Name; ;
                var r = await _roleManager.UpdateAsync(role);
                if (r.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View("Error", new ErrorViewModel() { RequestId = "Unknown error while editing role" });
                }
                
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel() { RequestId = ex.Message });
            }
        }

        // GET: RolesController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var role = _roleManager.Roles.Where(s => s.Id == id).Select(s => new RoleViewModel() { Id = s.Id, Name = s.Name }).FirstOrDefault();
            if (role == null)
            {
                return NotFound($"Role {id} was not found");
            }
            var users = _userManager.Users.ToList();
            List<string> myusers = new List<string>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    myusers.Add(user.UserName);
                }
            }

            ViewBag.myusers = myusers;
            return View(role);
        }

        // POST: RolesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, RoleViewModel collection)
        {
            try
            {
                IdentityRole role = await _roleManager.FindByIdAsync(id);
                if (role == null) return NotFound($"Role {id} was not found");
               
                var r = await _roleManager.DeleteAsync(role);
                if (r.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View("Error", new ErrorViewModel() { RequestId = "Unknown error while deleteing role" });
                }

            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel() { RequestId = ex.Message });
            }
        }

        public async Task<IActionResult> Users(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound("role");
            var users = _userManager.Users.ToList();

            List<string> myusers = new List<string>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    myusers.Add(user.Id);
                }
            }

            ViewBag.myusers = myusers;
            ViewBag.role = role;

            return View(users);
        }

        public async Task<IActionResult> adduser(string role, string user)
        {
            var u = await _userManager.FindByIdAsync(user);
            if (u == null) return NotFound("User");
            var r = await _roleManager.FindByIdAsync(role);
            if (r == null) return NotFound("Role");
            await _userManager.AddToRoleAsync(u, r.Name);
            await _signinmanager.RefreshSignInAsync(u);                         // refresh the user roles by recreating the cookie
            return RedirectToAction("users", new { Id = r.Id });
        }

        public async Task<IActionResult> removeuser(string role, string user)
        {
            var u = await _userManager.FindByIdAsync(user);
            if (u == null) return NotFound("User");
            var r = await _roleManager.FindByIdAsync(role);
            if (r == null) return NotFound("Role");
            await _userManager.RemoveFromRoleAsync(u, r.Name);
            await _signinmanager.RefreshSignInAsync(u);                         // refresh the user roles by recreating the cookie
            return RedirectToAction("users", new { Id = r.Id });
        }
    }
}
