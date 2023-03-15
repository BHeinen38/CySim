using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CySim.Data;
using CySim.Models;
using CySim.Models.TeamRegistration;
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

        [HttpPost]
        public IActionResult Create(TeamRegistration teamRegistration)
        {
            //if (_vroomDbContext.Makes.Where(x => x.Name == make.Name).Any())
            //    throw new Exception("Sorry this username already exist"); //this is where you will want to implement you username already exist


            if (ModelState.IsValid)
            {
                _context.Add(teamRegistration);
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
            if (ModelState.IsValid)
            {
                _context.Update(registration);
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
                    registration.SpotsTaken++;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User2 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User2 = name;
                    registration.SpotsTaken++;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User3 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User3 = name;
                    registration.SpotsTaken++;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User4 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User4 = name;
                    registration.SpotsTaken++;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User5 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User5 = name;
                    registration.SpotsTaken++;
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                else if (registration.User6 == null)
                {
                    _logger.LogInformation(name + " just joined the team " + teamRegistration.TeamName);
                    registration.User6 = name;
                    registration.SpotsTaken++;
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