using Microsoft.AspNetCore.Mvc;

namespace SelfImprov.Features.Home
{
    public class HomeIndex
    {
        [Route("/")]
        public class HomeController : Controller
        {
            public IActionResult Index() => RedirectToAction("List", "Goals");
            
            [Route("Error/{code?}")]
            public IActionResult Error(int? code)
            {
                if (code == 404) return View("404");
                if (code == 403) return View("403");
                
                return View(code);
            }
        }
    }
}
