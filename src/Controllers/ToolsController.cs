using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        public IActionResult Base64EncoderDecoder()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Base64Encoder([FromBody] object text)
        {
            if (text == null)
                return Json("Please Enter an Input!");
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text.ToString());
            var encodedText = System.Convert.ToBase64String(plainTextBytes);
            return Json(encodedText);
        }

        [HttpPost]
        public IActionResult Base64Decoder([FromBody] object text)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(text.ToString());
                var decodedText = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                return Json(decodedText);
            }
            catch (System.FormatException)
            {
                return Problem(statusCode: 400, title: "Please Enter Valid Base64 Text!");
            }
        }
    }
}