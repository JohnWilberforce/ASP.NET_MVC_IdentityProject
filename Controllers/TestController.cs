using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityFromScratch.Controllers
{

    public class TestController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "RequireUserManager")]
        public IActionResult RequireManager()
        {
            return View("Success");
        }
        [Authorize(Policy="RequireUserWalters")]
        public IActionResult RequireBrian()
        {
            return View("Success");
        }
        [Authorize(Policy = "RequireUserCarol")]
        public IActionResult RequireCarol()
        {
            return View("Success");
        }
        [Authorize(Policy = "Require18OrOlder")]
        public IActionResult Require18()
        {
            return View("Success");
        }
        [Authorize(Policy = "Require21OrOlder")]
        public IActionResult Require21()
        {
            return View("Success");
        }
        [Authorize(Policy = "Require55OrOlder")]
        public IActionResult Require55()
        {
            return View("Success");
        }
        [Authorize(Policy = "RequireFL")]
        public IActionResult RequireFL()
        {
            return View("Success");
        }
        [Authorize(Policy = "RequireGA")]
        public IActionResult RequireGA()
        {
            return View("Success");
        }

        [Authorize(Roles ="Super")]
        public IActionResult RequireSuper()
        {
            return View("Success");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult RequireAdmin()
        {
            return View("Success");
        }
        [Authorize]
        public IActionResult RequireAuthenticated()
        {
            return View("Success");
        }
    }
}
