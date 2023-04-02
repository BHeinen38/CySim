using System;
using System.Collections.Generic;
using System.Text;
using CySim.Models;
using CySim.Models.TeamRegistration;
using CySim.Models.Scenario;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CySim.Models.Tutorial;
using System.Linq;

namespace CySim.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //this is not being used at the moment but we might extend Identity user so this is what will do that.
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Scenario> Scenarios { get; set; }

        public DbSet<TeamRegistration> TeamRegistrations { get; set; }

        public DbSet<Tutorial> Tutorials { get; set; }

        public IEnumerable<TeamRegistration> Leaders()
        {
            return TeamRegistrations.OrderByDescending(x => x.Score);
        }
        public IEnumerable<TeamRegistration> Leaders(int count)
        {
            return TeamRegistrations.OrderByDescending(x => x.Score).Take(count);
        }
    }
}

