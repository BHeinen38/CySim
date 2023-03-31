using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CySim.Data;
using CySim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CySim.Models.ScoreBoardModels;
using CySim.Models.TeamRegistration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CySim.Controllers.ScoreBoardController
{
    public class ScoreBoardController : Controller
    {
        private readonly ILogger<ScoreBoardController> _logger;
        private readonly ApplicationDbContext _context;

        public ScoreBoardController(ILogger<ScoreBoardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult ScoreBoard()
        {
            return View(_context.TeamRegistrations.ToList().OrderByDescending(x => x.Score));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);

            if (registration == null)
                return NotFound();


            return View(registration);

        }

        [HttpPost]
        public IActionResult Edit(TeamRegistration registration)
        {
            var teamRegistration = _context.TeamRegistrations.Find(registration.Id);

            teamRegistration.Score = registration.Score;
            teamRegistration.Usability = registration.Usability;
            teamRegistration.Flags = registration.Flags;

            _context.Update(teamRegistration);
            _context.SaveChanges();
            return RedirectToAction(nameof(ScoreBoard));
            
           
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

