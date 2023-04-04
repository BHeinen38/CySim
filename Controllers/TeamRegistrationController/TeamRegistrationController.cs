using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Xml.Linq;
using CySim.Data;
using CySim.Models;
using CySim.Models.Scenario;
using CySim.Models.TeamRegistration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CySim.Controllers.TeamRegistrationController
{
    public class TeamRegistrationController : Controller
    {
        private readonly ILogger<TeamRegistrationController> _logger;

        private readonly ApplicationDbContext _context;

        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public TeamRegistrationController(ILogger<TeamRegistrationController> logger, ApplicationDbContext context, SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index()
        { 
            return View(_context.TeamRegistrations.ToList());
        }

        [HttpGet]
        public IActionResult TeamRegUserAlreadyIn(string name)
        {
            return View(name);
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
        public async Task<IActionResult> Create(TeamRegistration teamRegistration, IFormFile file)
        {

            var fileName = "";

            if (file == null)
                fileName = "DefaultImage.png";
            else
                fileName = file.FileName;


            foreach (var item in _context.TeamRegistrations)
            {
                if (item.TeamCreator == User.Identity.Name)
                {
                    _logger.LogInformation(User.Identity.Name + "Has Already created a team");
                    //this is where we will need to throw an error message showing that the name is already in a team
                    return RedirectToAction(nameof(Index));
                }
            }

            if (_context.TeamRegistrations.Any(x => x.FileName == fileName) && fileName != "DefaultImage.png")
            {
                ViewData["errors"] = "Sorry this file name already exist";
                return View();
            }
            if (file != null)
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

            if (User.IsInRole("Admin"))
            {
                registration.IsRed = teamRegistration.IsRed;
                registration.AvailSpots = 6;
            }
            else if(User.IsInRole("Blue Team"))
            {
                registration.IsRed = false;
                registration.User1 = User.Identity.Name;
                registration.AvailSpots = 5;
            }
            else if (User.IsInRole("Red Team"))
            {
                registration.IsRed = true;
                registration.User1 = User.Identity.Name;
                registration.AvailSpots = 5;
            }
            if (ModelState.IsValid)
            {
                registration.TeamCreator = User.Identity.Name;
                if (User.IsInRole("Blue Team") || User.IsInRole("Red Team"))
                {
                    IdentityUser identity =  GetIdentityUser(registration.TeamCreator);
                    await _userManager.AddToRoleAsync(identity, "Team User");
                    IdentityRole role = _context.Roles.Where(x => x.Name == "Team User").First();
                    await _roleManager.UpdateAsync(role);
                    

                }
                IdentityUser identityUser = _context.Users.Where(x => x.Email == User.Identity.Name).First();
                await _signInManager.RefreshSignInAsync(identityUser);
                _context.Add(registration);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            return View(teamRegistration);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);
            IdentityUser identity = null;
            if (registration == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Team User") || User.IsInRole("Admin"))
            {
                if(registration.User1 != null)
                {
                    identity = GetIdentityUser(registration.User1);
                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                }
                if (registration.User2 != null)
                {
                    identity = GetIdentityUser(registration.User2);
                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                }
                if (registration.User3 != null)
                {
                    identity = GetIdentityUser(registration.User3);
                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                }
                if (registration.User4 != null)
                {
                    identity = GetIdentityUser(registration.User4);
                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                }
                if (registration.User5 != null)
                {
                    identity = GetIdentityUser(registration.User5);
                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                }
                if (registration.User6 != null)
                {
                    identity = GetIdentityUser(registration.User6);
                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                }
            }

            await _userManager.UpdateAsync(identity);
            IdentityRole role = _context.Roles.Where(x => x.Name == "Team User").First();
            await _roleManager.UpdateAsync(role);
            IdentityUser identityUser = _context.Users.Where(x => x.Email == User.Identity.Name).First();
            await _signInManager.RefreshSignInAsync(identityUser);
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
        public async Task<IActionResult> Edit(TeamRegistration registration)
        {
            var startRegistration = _context.TeamRegistrations.Find(registration.Id);
            string[] tempUser = new string[6] { startRegistration.User1, startRegistration.User2, startRegistration.User3, startRegistration.User4,
            startRegistration.User5, startRegistration.User6};

            IdentityUser identity = null;

            if (ModelState.IsValid)
            {
                string[] Users = new string[6];
                var teamRegistration = CySim.HelperFunctions.TeamRegistrationHelper.GetEditTeamRegistration(registration, startRegistration, out Users);

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

                    for(int i =0; i < Users.Length; i++)
                    {
                        if (Users[i] == null)
                        {
                            continue;
                        }
                        else if (Users[i] != "Removed")
                        {
                            identity = GetIdentityUser(Users[i]);
                            await _userManager.AddToRoleAsync(identity, "Team User");
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    identity = GetIdentityUser(tempUser[0]);
                                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                                    break;
                                case 1:
                                    identity = GetIdentityUser(tempUser[1]);
                                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                                    break;
                                case 2:
                                    identity = GetIdentityUser(tempUser[2]);
                                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                                    break;
                                case 3:
                                    identity = GetIdentityUser(tempUser[3]);
                                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                                    break;
                                case 4:
                                    identity = GetIdentityUser(tempUser[4]);
                                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                                    break;
                                case 5:
                                    identity = GetIdentityUser(tempUser[5]);
                                    await _userManager.RemoveFromRoleAsync(identity, "Team User");
                                    break;
                            }
                        }
                    }

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
                await _userManager.UpdateAsync(identity);
                IdentityRole role = _context.Roles.Where(x => x.Name == "Team User").First();
                await _roleManager.UpdateAsync(role);
                IdentityUser identityUser = _context.Users.Where(x => x.Email == User.Identity.Name).First();
                await _signInManager.RefreshSignInAsync(identityUser);
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
        public async Task<IActionResult> Join(TeamRegistration teamRegistration)
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
                        return View(nameof(TeamRegUserAlreadyIn), name);
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
                    if (User.IsInRole("Blue Team") || User.IsInRole("Red Team"))
                    {
                        IdentityUser identity =  GetIdentityUser(User.Identity.Name);
                        await _userManager.AddToRoleAsync(identity, "Team User");
                        await _userManager.UpdateAsync(identity);
                        IdentityRole role = _context.Roles.Where(x => x.Name == "Team User").First();
                        await _roleManager.UpdateAsync(role);
                        await _signInManager.RefreshSignInAsync(identity);
                    }
                        

                    _logger.LogInformation(name + " just joined the team " + registration.TeamName);
                    _context.Update(registration);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }

            }
            return View(teamRegistration);
        }

        private IdentityUser GetIdentityUser(string email)
        {
            IdentityUser identity = _context.Users.Where(x => x.Email == email).First();
            return identity;
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}