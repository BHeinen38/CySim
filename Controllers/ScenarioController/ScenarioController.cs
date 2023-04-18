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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CySim.Models.Scenario;
using System.IO.Abstractions;


namespace CySim.Controllers
{
    [Authorize]
    public class ScenarioController : Controller
    {
        private readonly ILogger<ScenarioController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IFileSystem _fileSystem;

        public ScenarioController(ILogger<ScenarioController> logger, ApplicationDbContext context, IFileSystem fileSystem)
        {
            _logger = logger;
            _context = context;
            _fileSystem = fileSystem;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Scenarios.OrderBy(x => x.isRed).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, String Description, bool isRed)
        {
            TempData["errors"] = "";

            if (file == null)
            {
                _logger.LogError("Scenario Create: No file was uploaded");
                TempData["errors"] = "No file was uploaded";
                return RedirectToAction(nameof(Create));
            }
            if (Description == null)
            {
                _logger.LogError("Scenario Create: No Description was entered");
                TempData["errors"] = "No description was provided";
                return RedirectToAction(nameof(Create));
            }

            var fileName = file.FileName;

            if (await _context.Scenarios.AnyAsync(x => x.FileName == fileName))
            {
                _logger.LogError("Scenario Create: FileName of uploaded file matched another scenario");
                TempData["errors"] = "Sorry this file name already exist";
                return RedirectToAction(nameof(Create));
            }

            var filePath = Path.Combine("Documents/Scenario", fileName);

            using (var stream = _fileSystem.FileStream.New(Path.Combine("wwwroot", filePath), FileMode.Create))
            {
                await file.CopyToAsync(stream);
                _logger.LogInformation("Scenario Create: File was uploaded to Documents/Scenario");
            }

            var scenario = new Scenario()
            {
                FileName = fileName,
                FilePath = filePath,
                Description = Description,
                isRed = isRed,
            };

            if (ModelState.IsValid)
            {
                _context.Add(scenario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Scenario Create: New database entry created");

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Create));
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario == null)
            {
                _logger.LogError("Scenario Delete on id = " + id + ": No scenario has id = " + id);
                return NotFound();
            }

            var fileName = Path.Combine("wwwroot", scenario.FilePath);

            if (ModelState.IsValid && _fileSystem.File.Exists(fileName))
            {
                _fileSystem.File.Delete(fileName);
                _context.Scenarios.Remove(scenario);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Scenario Delete on id = " + id + ": " + fileName + " was deleted and database entry removed");
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario == null)
            {
                _logger.LogError("Scenario Edit on id = " + id + ": No scenario has id = " + id);
                return NotFound();
            }

            return View(scenario);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, IFormFile file, String FileName, String Description, bool isRed)
        {
            // HTML Form Error Handling 
            TempData["errors"] = "";

            var scenario = await _context.Scenarios.FindAsync(id);
            if (scenario == null)
            {
                _logger.LogError("Scenario Edit on id = " + id + ": No scenario has id = " + id);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Scenario Edit on id = " + id + ": Model state was invalid");

                var errorMessage = string.Join("; ", ModelState.Values
                                    .SelectMany(x => x.Errors)
                                    .Select(x => x.ErrorMessage));
                _logger.LogError("Model state errors messages: " + errorMessage);

                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (FileName == null)
            {
                _logger.LogError("Scenario Edit on id = " + id + ": No FileName was entered");
                TempData["errors"] = "No file name was provided";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (await _context.Scenarios.AnyAsync(x => x.Id != id && x.FileName == FileName))
            {
                _logger.LogError("Scenario Edit on id = " + id + ": FileName matched another scenario");
                TempData["errors"] = "This file name is already used by another scenario";
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            if (Description == null)
            {
                _logger.LogError("Scenario Edit on id = " + id + ": No Description was entered");
                TempData["errors"] = "No description was provided";
                return RedirectToAction(nameof(Edit), new { id = id });
            }


            var CurrFile = Path.Combine("wwwroot", scenario.FilePath);
            var NewFile = Path.Combine("wwwroot/Documents/Scenario", FileName);

            // Replace file contents
            if (file != null)
            {
                using (var stream = _fileSystem.FileStream.New(CurrFile, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _logger.LogInformation("Scenario Edit on id = " + id + ": File contents were replaced by uploaded file");
            }

            // Rename file
            if (CurrFile != NewFile)
            {
                _fileSystem.File.Move(CurrFile, NewFile);
                _logger.LogInformation("Scenario Edit on id = " + id + ": File was renamed");
            }

            // Update scenario variable
            scenario.FileName = FileName;
            scenario.FilePath = Path.Combine("Documents/Scenario", FileName);
            scenario.Description = Description;
            scenario.isRed = isRed;

            _context.Scenarios.Update(scenario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Scenario Edit on id = " + id + ": Database entry was updated");

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