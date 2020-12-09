using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KolcuogluNet.Models;

namespace KolcuogluNet.Controllers
{
    public class ToolsController : Controller
    {
        private readonly ILogger<ToolsController> _logger;

        public ToolsController(ILogger<ToolsController> logger)
        {
            _logger = logger;
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult RandomColorGenerator()
        {
            return View();
        }

        public IActionResult Counter()
        {
            return View();
        }

        public IActionResult ByteCalculator()
        {
            return View();
        }
    }
}