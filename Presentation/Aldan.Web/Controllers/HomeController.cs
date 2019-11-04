using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Aldan.Services.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Aldan.Web.Models;

namespace Aldan.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWorkflowMessageService _workflowMessageService;

        public HomeController(IWorkflowMessageService workflowMessageService)
        {
            _workflowMessageService = workflowMessageService;
        }

        public IActionResult Index()
        {
             return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}