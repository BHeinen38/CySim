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
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

        //Errors
        //we can get rid of this entire functions 
        [HttpGet]
        public IActionResult UserExistError(string name)
        {
            return View(name);
        }

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
            {
                fileName = "DefaultImage.png";
            }
            else
            {
                if (file.FileName == "DefaultImage.png")
                {
                    TempData["errors"] = "Error: Cannot upload a file with same name as default";
                    _logger.LogInformation("Error: Cannot upload a file with same name as default");
                    return RedirectToAction(nameof(Create), new { Id = teamRegistration.Id });
                }

                fileName = file.FileName;
            }

            foreach (var item in _context.TeamRegistrations)
            {
                //TODO: Test to see if this check is needed 
                if (item.TeamCreator == User.Identity.Name && !User.IsInRole("Admin"))
                {
                    _logger.LogInformation("Error: {item.TeamCreator} has already created a team", item.TeamCreator);
                    TempData["errors"] = "Sorry, this user has already created a team.";
                    //TODO: This is the one that might not be needed
                    return RedirectToAction(nameof(Index));
                }
                if (item.TeamName == teamRegistration.TeamName)
                {
                    _logger.LogInformation("Error: Team called {item.TeamName} already exists", item.TeamName);
                    //TODO: Return a TeamExistError
                    TempData["errors"] = "Sorry, a team with that name already exists.";
                    //be wary of this test if works lol
                    return RedirectToAction(nameof(Create), new { Id = teamRegistration.Id });
                }
                if (item.FileName == fileName && fileName != "DefaultImage.png")
                {
                    _logger.LogInformation("Error: File called {item.FileName} already exists", item.FileName);
                    TempData["errors"] = "Sorry, a file with that name already exists.";
                    //TODO: Return a Filename Exist Error
                    return RedirectToAction(nameof(Create), new { Id = teamRegistration.Id });
                }
            }

            if (file != null)
            {
                using (var stream = new FileStream(Path.Combine("wwwroot/Documents/Images", fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            var registration = new TeamRegistration();

            registration.FileName = fileName;
            registration.FilePath = Path.Combine("Documents/Images", fileName);
            registration.TeamName = teamRegistration.TeamName;

            if (User.IsInRole("Admin"))
            {
                registration.IsRed = teamRegistration.IsRed;
                registration.AvailSpots = 6;
            }
            else if (User.IsInRole("Blue Team"))
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
                    IdentityUser identity = GetIdentityUser(registration.TeamCreator);
                    await _userManager.AddToRoleAsync(identity, "Team User");
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
        [Authorize(Roles = "Admin,Team User")]
        public async Task<IActionResult> Delete(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);

            string[] users = new string[6] { registration.User1, registration.User2, registration.User3, registration.User4,
            registration.User5, registration.User6};

            IdentityUser identity = null;

            if (registration == null)
            {
                //TODO: Return registration not found error
                TempData["errors"] = "Team registration not found. Try registering a team.";
                _logger.LogInformation("Error: Team registration not found");
                return RedirectToAction(nameof(Edit), new { id = registration.Id });
            }

            if (User.IsInRole("Team User") || User.IsInRole("Admin"))
            {
                for (var i = 0; i < users.Length; i++)
                {
                    if (users[i] != null)
                    {
                        identity = GetIdentityUser(users[i]);
                        await _userManager.RemoveFromRoleAsync(identity, "Team User");
                    }
                }
            }
            if (registration.FileName != "DefaultImage.png")
                System.IO.File.Delete(Path.Combine("wwwroot/", registration.FilePath));

            if (identity != null)
                await _userManager.UpdateAsync(identity);

            IdentityUser identityUser = _context.Users.Where(x => x.Email == User.Identity.Name).First();
            await _signInManager.RefreshSignInAsync(identityUser);
            _context.TeamRegistrations.Remove(registration);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Authorize(Roles = "Admin,Team User")]
        public IActionResult Edit(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);

            //TODO: Same Return Registration Error
            if (registration == null)
            {
                TempData["errors"] = "Team registration not found. Try registering a team.";
                _logger.LogInformation("Error: Team registration not found");
                return RedirectToAction(nameof(Edit), new { id = registration.Id });
            }

            return View(registration);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Team User")]
        public async Task<IActionResult> Edit(TeamRegistration registration, IFormFile file)
        {
            var startRegistration = _context.TeamRegistrations.Find(registration.Id);
            string[] tempUser = new string[6] { startRegistration.User1, startRegistration.User2, startRegistration.User3, startRegistration.User4,
            startRegistration.User5, startRegistration.User6};

            string[] curUser = new string[6] { registration.User1, registration.User2, registration.User3, registration.User4,
            registration.User5, registration.User6};

            IdentityUser identity = null;
            var fileName = "";

            if (file == null)
                fileName = startRegistration.FileName;
            else
            {
                //User cannot create a file in the same name as our default
                if (file.FileName == "DefaultImage.png")
                {
                    TempData["errors"] = "Error: Cannot upload a file with same name as default";
                    _logger.LogInformation("Error: Cannot upload a file with same name as default");
                    return RedirectToAction(nameof(Edit), new { Id = registration.Id });
                }

                fileName = file.FileName;
            }

            //Flag - comparing i var with the rest of the array. then doing next increment of i and comparing rest again. Repeat til i is at second to last count
            for (int i = 0; i < curUser.Length; i++)
            {
                for (int j = 0; j < curUser.Length; j++)
                {
                    //if these two are different objects and match, that's a no no
                    if (i != j && curUser[i] == curUser[j] && curUser[i] != null && curUser[j] != null)
                    {
                        TempData["errors"] = "Error: A file with this name already exists";
                        _logger.LogInformation("Error: A file with this name already exists");
                        return RedirectToAction(nameof(Edit), new { id = registration.Id });
                    }
                }
            }

            if (_context.TeamRegistrations.Any(x => x.FileName == fileName) && fileName != startRegistration.FileName && fileName != "DefaultImage.png")
            {
                TempData["errors"] = "Error: A file with this name already exists";
                _logger.LogInformation("Error: A file with this name already exists");
                return RedirectToAction(nameof(Index));
            }

            if (System.IO.File.Exists(Path.Combine("wwwroot/", startRegistration.FilePath)) && startRegistration.FileName != fileName && startRegistration.FileName != "DefaultImage.png")
                System.IO.File.Delete(Path.Combine("wwwroot/", startRegistration.FilePath));

            if (file != null)
            {
                using (var stream = new FileStream(Path.Combine("wwwroot/Documents/Images", fileName), FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            if (ModelState.IsValid)
            {
                string[] Users = new string[6];
                var teamRegistration = CySim.HelperFunctions.TeamRegistrationHelper.GetEditTeamRegistration(registration, startRegistration, out Users);

                for (int i = 0; i < Users.Length; i++)
                {
                    if (Users[i] == "There is no user in database")
                    {
                        TempData["errors"] = "Error: No users found in database";
                        _logger.LogInformation("Error: No users found in database");
                        return RedirectToAction(nameof(Edit), new { id = registration.Id });
                    }
                }
                teamRegistration.TeamName = registration.TeamName;

                for (int i = 0; i < Users.Length; i++)
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

                teamRegistration.FileName = fileName;
                teamRegistration.FilePath = Path.Combine("Documents/Images", fileName);

                if (User.IsInRole("Admin"))
                    teamRegistration.IsRed = registration.IsRed;

                if (User.IsInRole("Blue Team"))
                    teamRegistration.IsRed = false;

                if (User.IsInRole("Red Team"))
                    teamRegistration.IsRed = true;

                if (identity != null)
                    await _userManager.UpdateAsync(identity);

                IdentityUser identityUser = _context.Users.Where(x => x.Email == User.Identity.Name).First();
                await _signInManager.RefreshSignInAsync(identityUser);
                _context.Update(teamRegistration);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(registration);
        }

        [HttpGet]
        [Authorize(Roles = "Red Team,Blue Team")]
        public IActionResult Join(int id)
        {
            var registration = _context.TeamRegistrations.Find(id);

            //TODO: Return Registration not found Error 
            if (registration == null)
            {
                TempData["errors"] = "Team registration not found. Try registering a team.";
                _logger.LogInformation("Error: Team registration not found");
                return RedirectToAction(nameof(Join), new { id = registration.Id });
            }

            return View(registration);
        }

        [HttpPost]
        [Authorize(Roles = "Red Team,Blue Team")]
        public async Task<IActionResult> Join(TeamRegistration teamRegistration)
        {
            if (ModelState.IsValid)
            {
                var name = User.Identity.Name;
                var startRegistration = _context.TeamRegistrations.Find(teamRegistration.Id);

                //one user can only join one team
                foreach (var item in _context.TeamRegistrations)
                {
                    if (item.User1 == name || item.User2 == name || item.User3 == name || item.User4 == name || item.User5 == name
                        || item.User6 == name)
                    {
                        _logger.LogInformation(name + " is already in " + item.TeamName);
                        TempData["errors"] = "You are already in " + item.TeamName + "; Users can only join 1 team.";
                        return RedirectToAction(nameof(Join));
                    }
                }

                var registration = CySim.HelperFunctions.TeamRegistrationHelper.GetJoinTeamRegistration(teamRegistration, startRegistration, name);

                if (registration.TeamName == "Team already has 6 users")
                {
                    TempData["errors"] = "Sorry, this team already has the maximum of 6 members!";
                    _logger.LogInformation("Error: User could not join a full team");
                    return RedirectToAction(nameof(Join), new { id = registration.Id });
                }
                else
                {
                    if (User.IsInRole("Blue Team") || User.IsInRole("Red Team"))
                    {
                        IdentityUser identity = GetIdentityUser(User.Identity.Name);
                        await _userManager.AddToRoleAsync(identity, "Team User");
                        await _userManager.UpdateAsync(identity);
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