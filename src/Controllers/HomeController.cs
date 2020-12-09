using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KolcuogluNet.Models;

namespace KolcuogluNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Ozgecmis")]
        public IActionResult Ozgecmis()
        {
            return View();
        }

        [Route("/Portfoy/Liste")]
        public IActionResult Portfoy()
        {
            return View();
        }

        [Route("/wp-content.php")]
        [Route("/wp-login.php")]
        [Route("/.env")]
        public string Wordpress(string parameter)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Hey! WordPress veya benzeri bir sistem kullanmıyorum :) Lütfen boşuna uğraşmayın!\n")
              .AppendLine("Hey! I don't use Wordpress or similar system :) Please don't waste your time!\n")
              .AppendLine("Saygılar - Regards,")
              .AppendLine("Kemal KOLCUOGLU");
            return sb.ToString();
        }

        [Route("/robots.txt")]
        public string RobotsTxt()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("User-agent: *")
              .AppendLine("Disallow:");

            return sb.ToString();
        }
    }
}
