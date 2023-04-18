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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace CySim.Controllers
{
    [Authorize]
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(IFormFile file, String Description, bool isRed, bool isGameType)
        {
            TempData["errors"] = "";

            if (file == null)
            {
                TempData["errors"] = "No file was uploaded";
                return View();
            }
            if (Description == null)
            {
                TempData["errors"] = "No description was provided";
                return View();
            }

            var fileName = file.FileName;

            if (_context.Tutorials.Any(x => x.FileName == fileName))
            {
                TempData["errors"] = "Sorry this file name already exists";
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete([FromRoute] int id)
        {
            var tutorial = _context.Tutorials.Find(id);
            if (tutorial == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var fileName = Path.Combine("wwwroot/", tutorial.FilePath);

            if (ModelState.IsValid && System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
                _context.Tutorials.Remove(tutorial);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit([FromRoute] int id)
        {
            var tutorial = _context.Tutorials.Find(id);
            if (tutorial == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(tutorial);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, IFormFile file, String FileName, String Description, bool isRed, bool isGametype)
        {
            TempData["errors"] = "";

            var tutorial = _context.Tutorials.Find(id);
            if (tutorial == null)
            {

                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {

                var errorMessage = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));
                _logger.LogError("Model state errors messages: " + errorMessage);

                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (FileName == null)
            {
                TempData["errors"] = "No file name was provided";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (_context.Tutorials.Any(x => x.Id != id && x.FileName == FileName))
            {
                TempData["errors"] = "This file name is already used by another scenario";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (Description == null)
            {
                TempData["errors"] = "No description was provided";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            var CurrFile = Path.Combine("wwwroot", tutorial.FilePath);
            var NewFile = Path.Combine("wwwroot/Documents/Tutorial", FileName);

            if (file != null)
            {
                using (var stream = new FileStream(CurrFile, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            if (CurrFile != NewFile)
            {
                System.IO.File.Move(CurrFile, NewFile);
            }

            tutorial.FileName = FileName;
            tutorial.FilePath = Path.Combine("Documents/Tutorial", FileName);
            tutorial.Description = Description;
            tutorial.isRed = isRed;
            tutorial.isGameType = isGametype;

            _context.Tutorials.Update(tutorial);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}