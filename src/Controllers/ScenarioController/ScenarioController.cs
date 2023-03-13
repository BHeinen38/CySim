using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CySim.Data;
using CySim.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CySim.Models.Scenario;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CySim.Controllers
{
    public class ScenarioController : Controller
    {
        private readonly ILogger<ScenarioController> _logger;
        private readonly ApplicationDbContext _context;

        public ScenarioController(ILogger<ScenarioController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Scenarios.OrderBy(x => x.isRed).ToList());
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
        public IActionResult Create(IFormFile file, String Description, bool isRed)
        {
            ViewData["errors"] = "";

            if(file == null) 
            {
                ViewData["errors"] = "No file was uploaded"; 
                return View();
            }
            if(Description == null) 
            {
                ViewData["errors"] = "No description was provided"; 
                return View();
            }
            
            var fileName = file.FileName; 
            
            if (_context.Scenarios.Any(x => x.FileName == fileName))
            {
                ViewData["errors"] = "Sorry this file name already exist";
                return View();
            }

            using (var stream = new FileStream(Path.Combine("wwwroot/Documents/Scenario", fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var scenario = new Scenario() 
            {
                FileName = fileName,
                FilePath = Path.Combine("Documents/Scenario", fileName),
                Description = Description,
                isRed = isRed,
            };

            if (ModelState.IsValid)
            {
                _context.Add(scenario);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogInformation("User made an error at scenario controller");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

