using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECApp.Controllers
{
    public class MyAppController : Controller
    {
        // GET
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}