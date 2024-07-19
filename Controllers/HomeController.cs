using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Video_Catalogue_Challenge.Models;

namespace Video_Catalogue_Challenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _mediaFolderPath;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _mediaFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "media");
            if (!Directory.Exists(_mediaFolderPath))
            {
                Directory.CreateDirectory(_mediaFolderPath);
            }
        }

        public IActionResult Index()
        {
            return View();

        }

        public IActionResult Upload()
        {
            _logger.LogInformation("On Upload page.");
            return PartialView("Upload");
        }

        public IActionResult Catalogue()
        {
            _logger.LogInformation("Retrieving video files.");
            var videos = Directory.EnumerateFiles(_mediaFolderPath, "*.mp4")
                .Select(filePath => new Video
                {
                    Name = Path.GetFileName(filePath),
                    Size = new FileInfo(filePath).Length,
                    Path = "/media/" + Path.GetFileName(filePath)
                })
                .ToList();
            _logger.LogInformation("Retrieved {VideoCount} videos.", videos.Count);
            return PartialView("Catalogue", videos);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
