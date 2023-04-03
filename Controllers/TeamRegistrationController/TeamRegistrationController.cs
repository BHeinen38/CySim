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
using Microsoft.Data.SqlClient;
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
            registration.TeamName = teamRegistration.TeamName;
            registration.AvailSpots = 6;

            if (User.IsInRole("Admin"))
            {
                registration.IsRed = teamRegistration.IsRed;
            }
            if(User.IsInRole("Blue Team"))
            {
                registration.IsRed = false;
            }
            if (User.IsInRole("Red Team"))
            {
                registration.IsRed = true;
            }

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
            var startRegistration = _context.TeamRegistrations.Find(registration.Id);

            if (ModelState.IsValid)
            {
                var teamRegistration = CySim.HelperFunctions.TeamRegistrationHelper.GetEditTeamRegistration(registration, startRegistration);

                if (teamRegistration.User1 == "There is no user in database" || teamRegistration.User2 == "There is no user in database" || teamRegistration.User3 == "There is no user in database"
                    || teamRegistration.User4 == "There is no user in database" || teamRegistration.User5 == "There is no user in database" || teamRegistration.User6 == "There is no user in database")
                {
                    //this is where we will need to add a error page showing that the User does not exist
                    _logger.LogInformation("There is no user of this type username in the database");
                    return RedirectToAction(nameof(Index));
                }

                else
                {
                    teamRegistration.TeamName = registration.TeamName;
                    _logger.LogInformation("Successfully update the team");
                }


                if (User.IsInRole("Admin"))
                {
                    teamRegistration.IsRed = registration.IsRed;
                }
                if (User.IsInRole("Blue Team"))
                {
                    teamRegistration.IsRed = false;
                }
                if (User.IsInRole("Red Team"))
                {
                    teamRegistration.IsRed = true;
                }

                //We will need to update our available spots from edit
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
            if (ModelState.IsValid)
            {
                var name = User.Identity.Name;
                var startRegistration = _context.TeamRegistrations.Find(teamRegistration.Id);


                //one user can only join one team
                foreach(var item in _context.TeamRegistrations)
                {
                    if(item.User1 == name || item.User2 == name || item.User3 == name || item.User4 == name || item.User5 == name
                        || item.User6 == name)
                    {
                        _logger.LogInformation(name + " is already in " + item.TeamName);
                        //this is where we will need to throw an error message showing that the name is already in a team
                        return RedirectToAction(nameof(Index));
                    }
                }


                //if (name == registration.User1 || name == registration.User2 || name == registration.User3 || name == registration.User4 ||
                //    name == registration.User5 || name == registration.User6)
                //{
                //    _logger.LogInformation(name + " is already in " + registration.TeamName);
                //    return RedirectToAction(nameof(Index));
                //}
                var registration = CySim.HelperFunctions.TeamRegistrationHelper.GetJoinTeamRegistration(teamRegistration, startRegistration, name);
                
                if(registration.TeamName == "Team already has 6 users")
                {
                    _logger.LogInformation("Sorry this team already has 6 users");
                    //this is where we will need to create an error message showing that the team already has 6 users.
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogInformation(name + " just joined the team " + registration.TeamName);
                    _context.Update(registration);
                    _context.SaveChanges();

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