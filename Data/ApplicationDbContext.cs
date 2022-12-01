using System;
using System.Collections.Generic;
using System.Text;
using CySim.Models;
using CySim.Models.ScoreBoardModels;
using CySim.Models.TeamRegistration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CySim.Models.Tutorial;

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

        public DbSet<ScoreBoard> ScoreBoards { get; set; }

        public DbSet<TeamRegistration> TeamRegistrations { get; set; }

        public DbSet<Tutorial> Tutorials { get; set; }


    }
}

