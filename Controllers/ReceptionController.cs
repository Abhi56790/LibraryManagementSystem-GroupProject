using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Reception")]
    public class ReceptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
