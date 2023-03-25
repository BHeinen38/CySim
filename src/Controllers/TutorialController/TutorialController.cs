using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CySim.Controllers.TeamRegistrationController;
using CySim.Data;
using CySim.Models;
using CySim.Models.Scenario;
using CySim.Models.Tutorial;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace CySim.Controllers
{
    public class TutorialController : Controller
    {

        private readonly ILogger<TutorialController> _logger;
        private readonly ApplicationDbContext _context;

        public TutorialController(ILogger<TutorialController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Tutorials.ToList());
        }

        //we need two create becuase one  is to display form to user
        //other one is used as a submit to save data
        //HTTP Get Method
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(IFormFile file, String Description, bool isRed, bool isGameType)
        {
            ViewData["errors"] = "";

            if (file == null)
            {
                ViewData["errors"] = "No file was uploaded";
                return View();
            }
            if (Description == null)
            {
                ViewData["errors"] = "No description was provided";
                return View();
            }

            var fileName = file.FileName;

            if (_context.Tutorials.Any(x => x.FileName == fileName))
            {
                ViewData["errors"] = "Sorry this file name already exists";
                return View();
            }

            using (var stream = new FileStream(Path.Combine("wwwroot/Documents/Tutorial", fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var tutorial = new Tutorial()
            {
                FileName = fileName,
                FilePath = Path.Combine("Documents/Tutorial", fileName),
                Description = Description,
                isRed = isRed,
                isGameType = isGameType,
            };

            if (ModelState.IsValid)
            {
                _context.Add(tutorial);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogInformation("User made an error at tutorial controller");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
