using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CySim.Data;
using CySim.Models;
using CySim.Models.Scenario;
using CySim.Models.TeamRegistration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CySim.Controllers.TeamRegistrationController
{
    public class TeamRegistrationController : Controller
    {
        private readonly ILogger<TeamRegistrationController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;

        public TeamRegistrationController(ILogger<TeamRegistrationController> logger, ApplicationDbContext context, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.TeamRegistrations.ToList());
        }

        //we need two create becuase one  is to display form to user
        //other one is used as a submit to save data
        //HTTP Get Method
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Create(TeamRegistration teamRegistration)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        teamRegistration.TeamCreator = User.Identity.Name;
        //        _context.Add(teamRegistration);
        //        _context.SaveChanges();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(teamRegistration);
        //}

        [HttpPost]
        public IActionResult Create(TeamRegistration teamRegistration, IFormFile file)
        {

            var fileName = "";

            if (file == null)
                fileName = "DefaultImage.png";
            else
                fileName = file.FileName;


            if (_context.TeamRegistrations.Any(x => x.FileName == fileName) && fileName != "DefaultImage.png")
            {
                ViewData["errors"] = "Sorry this file name already exist";
                return View();
            }
            if(file != null)
            {
                using (var stream = new FileStream(Path.Combine("wwwroot/Documents/Images", fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            var registration = new TeamRegistration();

            //var registration = new TeamRegistration()
            //{
            //    FileName = fileName,
            //    FilePath = Path.Combine("Documents/Images", fileName),
            //    IsRed = teamRegistration.IsRed,
            //    TeamName = teamRegistration.TeamName
            //};

            registration.FileName = fileName;
            registration.FilePath = Path.Combine("Documents/Images", fileName);
            registration.IsRed = teamRegistration.IsRed;
            registration.TeamName = teamRegistration.TeamName;
            registration.AvailSpots = 6;
            

            if (ModelState.IsValid)
            {
                registration.TeamCreator = User.Identity.Name;
                _context.Add(registration);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(teamRegistration);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);
            if (registration == null)
            {
                return NotFound();
            }
            _context.TeamRegistrations.Remove(registration);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //HTTP GET
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

            if (ModelState.IsValid)
            {
                teamRegistration.TeamName = registration.TeamName;
                _context.Update(teamRegistration);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }


        //HTTP GET
        [HttpGet]
        public IActionResult Join(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);


            if (registration == null)
                return NotFound();


            return View(registration);

        }

        [HttpPost]
        public IActionResult Join(TeamRegistration teamRegistration)
        {
            //if (_vroomDbContext.Makes.Where(x => x.Name == make.Name).Any())
            //    throw new Exception("Sorry this username already exist"); //this is where you will want to implement you username already exist
            if (ModelState.IsValid)
            {
                var name = User.Identity.Name;
                var registration = _context.TeamRegistrations.Find(teamRegistration.Id);


                //one user can only join one team
                foreach(var item in _context.TeamRegistrations)
                {
                    if(item.User1 == name || item.User2 == name || item.User3 == name || item.User4 == name || item.User5 == name
                        || item.User6 == name)
                    {
                        _logger.LogInformation(name + " is already in " + item.TeamName);
                        return RedirectToAction(nameof(Index));
                    }
                }


                if (name == registration.User1 || name == registration.User2 || name == registration.User3 || name == registration.User4 ||
                    name == registration.User5 || name == registration.User6)
                {
                    _logger.LogInformation(name + " is already in " + teamRegistration.TeamName);
                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User1 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User1 = name;
                    registration.AvailSpots--;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User2 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User2 = name;
                    registration.AvailSpots--;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User3 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User3 = name;
                    registration.AvailSpots--;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User4 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User4 = name;
                    registration.AvailSpots--;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User5 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User5 = name;
                    registration.AvailSpots--;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User6 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User6 = name;
                    registration.AvailSpots--;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogInformation("Sorry this team already has 6 users");
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(teamRegistration);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}