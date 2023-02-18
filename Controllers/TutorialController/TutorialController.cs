using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CySim.Controllers.TeamRegistrationController;
using CySim.Data;
using CySim.Models;
using CySim.Models.Tutorial;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace CySim.Controllers.TutorialController
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
            return View(_context.Tutorials.OrderBy(x => x.isRed).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine("/Users/bailey/Desktop/CySim2/CySim/wwwroot/Documents/Tutorial", formFile.FileName);
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return Ok(new { count = files.Count, size, filePaths });
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
        public IActionResult Create(Tutorial tutorial)
        {
            //if (_vroomDbContext.Makes.Where(x => x.Name == make.Name).Any())
            //    throw new Exception("Sorry this username already exist"); //this is where you will want to implement you username already exist


            if (ModelState.IsValid)
            {
                _context.Add(tutorial);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(tutorial);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogInformation("User made an error at Tutrial controller");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}

