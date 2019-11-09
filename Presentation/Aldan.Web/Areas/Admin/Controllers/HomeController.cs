using Microsoft.AspNetCore.Mvc;

namespace Aldan.Web.Areas.Admin.Controllers
{
    public class HomeController:BaseAdminController
    {
        public  IActionResult Index()
        {
            return View();
        }
    }
}