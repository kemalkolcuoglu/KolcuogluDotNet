using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using KolcuogluNet.Models;
using System.Collections.Generic;

namespace KolcuogluNet.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;

        public BlogController(ILogger<BlogController> logger)
        {
            _logger = logger;
        }

        public IActionResult Liste()
        {
            List<Post> posts = JArray.Parse(System.IO.File.ReadAllText("wwwroot/post/posts.json")).ToObject<List<Post>>();
            ViewData["Posts"] = posts;
            return View();
        }

        public IActionResult Yazi(string slug)
        {
            if (slug == null)
                return RedirectToAction(nameof(Liste));
            slug = slug.ToLower();
            List<Post> posts = JArray.Parse(System.IO.File.ReadAllText("wwwroot/post/posts.json")).ToObject<List<Post>>();
            Post finded_post = posts.Find(x => x.Slug == slug);
            if(finded_post == null)
                return RedirectToAction(nameof(Liste));
            return View(finded_post);
        }
    }
}