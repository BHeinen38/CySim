using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CySim.Data;
using CySim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CySim.Models.Scenario;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CySim.Controllers
{
    [Authorize]
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

        [Authorize(Roles="Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles="Admin")]
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


        [Authorize(Roles="Admin")]
        [HttpPost]
        public IActionResult Delete([FromRoute] int id)
        {
            var scenario = _context.Scenarios.Find(id);
            if(scenario == null) 
            { 
                return RedirectToAction(nameof(Index));
            }
            
            var fileName = Path.Combine("wwwroot/", scenario.FilePath);
            
            if (ModelState.IsValid && System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
                _context.Scenarios.Remove(scenario);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles="Admin")]
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var scenario = _context.Scenarios.Find(id);
            if(scenario == null) 
            { 
                _logger.LogError("Scenario Edit on id = " + id + ": No scenario has id = " + id);
                return RedirectToAction(nameof(Index));
            }

            return View(scenario);
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public IActionResult Edit([FromRoute]int id, IFormFile file, String FileName, String Description, bool isRed)
        {
            // HTML Form Error Handling 
            ViewData["errors"] = "";

            var scenario = _context.Scenarios.Find(id);
            if(scenario == null) 
            { 
                _logger.LogError("Scenario Edit on id = " + id + ": No scenario has id = " + id);
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Scenario Edit on id of " + id + ": Model state was invalid");
                
                var errorMessage = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));
                _logger.LogError("Model state errors messages: " + errorMessage);

                ViewData["errors"] = "Model state is invalid";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if(FileName == null) 
            {
                _logger.LogError("Scenario Edit on id of " + id + ": No FileName was entered");
                ViewData["errors"] = "No file name was provided"; 
                return RedirectToAction(nameof(Edit), new { id = id });
            }
          
            if (_context.Scenarios.Any(x => x.Id != id && x.FileName == FileName))
            {
                _logger.LogError("Scenario Edit on id of " + id + ": FileName matched another scenario");
                ViewData["errors"] = "Sorry this file name is already used by another scenario";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if(Description == null) 
            {
                _logger.LogError("Scenario Edit on id of " + id + ": No Description was entered");
                ViewData["errors"] = "No description was provided"; 
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            

            var CurrFile = Path.Combine("wwwroot", scenario.FilePath);
            var NewFile = Path.Combine("wwwroot/Documents/Scenario", FileName);

            // Replace file contents
            if (file != null) 
            {
                using (var stream = new FileStream(CurrFile, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            // Rename file
            System.IO.File.Move(CurrFile, NewFile);

            // Update scenario variable
            scenario.FileName = FileName;
            scenario.FilePath = Path.Combine("Documents/Scenario", FileName); 
            scenario.Description = Description;
            scenario.isRed = isRed;
            
            _context.Scenarios.Update(scenario);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("User made an error at scenario controller");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
