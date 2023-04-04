using System;
using CySim.Controllers.TeamRegistrationController;
using CySim.Models.TeamRegistration;
using CySim.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace CySim.HelperFunctions
{
    public class TeamRegistrationHelper
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public TeamRegistrationHelper(ILogger<TeamRegistrationController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        private static string FindUser(string Email)
        {
            string connectionString = "Server=localhost;Database=CySim;User Id=sa;Password=cpre491p@ssword;Trusted_Connection=True;MultipleActiveResultSets=true;Integrated Security=False";
            string returnUser = null;
            if (Email != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("", connection))
                    {
                        command.CommandText = "SELECT * FROM [CySim].[dbo].[AspNetUsers] WHERE Email=@Email";
                        command.Parameters.AddWithValue("@Email", Email);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            returnUser = reader["Email"].ToString();
                        }
                    }
                    connection.Close();
                }
            }

            return returnUser;
        }
        public static int AddOrDenyUser(string oldUser, string newUser, string teamName)
        {
            int userInt = -1;
            var user = FindUser(newUser);
            if (newUser == null)
            {
                //_logger.LogInformation("User {user} has succesfully been removed from the team named {teamName}", user, teamName);
                userInt = 1;
                return userInt;

            }
            else
            {
                if (user == null)
                {
                    //_logger.LogError("Sorry that user does not exist");
                    userInt = 0;
                    return userInt;
                }
                else
                {
                    //_logger.LogInformation("User {user} has succesfully joined the team named {teamName}", user, teamName);
                    userInt = -1;
                    return userInt;
                }
            }
        }

        public static TeamRegistration GetEditTeamRegistration(TeamRegistration registration, TeamRegistration teamRegistration, out string[] Users)
        {
            Users = new string[6];
            teamRegistration.TeamName = registration.TeamName;

            if (teamRegistration.User1 != registration.User1)
            {
                if (AddOrDenyUser(teamRegistration.User1, registration.User1, teamRegistration.TeamName) == 1)
                {
                    teamRegistration.User1 = registration.User1;
                    teamRegistration.AvailSpots++;
                    Users[0] = "Removed";

                }
                else if (AddOrDenyUser(teamRegistration.User1, registration.User1, teamRegistration.TeamName) == -1)
                {
                    teamRegistration.User1 = registration.User1;
                    teamRegistration.AvailSpots--;
                    Users[0] = registration.User1;
                }
                else
                {
                    teamRegistration.User1 = "There is no user in database";
                }
            }
            if (teamRegistration.User2 != registration.User2)
            {
                if (AddOrDenyUser(teamRegistration.User2, registration.User2, teamRegistration.TeamName) == 1)
                {
                    teamRegistration.User2 = registration.User2;
                    teamRegistration.AvailSpots++;
                    Users[1] = "Removed";

                }
                else if (AddOrDenyUser(teamRegistration.User2, registration.User2, teamRegistration.TeamName) == -1)
                {
                    teamRegistration.User2 = registration.User2;
                    teamRegistration.AvailSpots--;
                    Users[1] = registration.User2;
                }
                else
                {
                    teamRegistration.User2 = "There is no user in database";
                }
            }
            if (teamRegistration.User3 != registration.User3)
            {
                if (AddOrDenyUser(teamRegistration.User3, registration.User3, teamRegistration.TeamName) == 1)
                {
                    teamRegistration.User3 = registration.User3;
                    teamRegistration.AvailSpots++;
                    Users[2] = "Removed";
                }
                else if (AddOrDenyUser(teamRegistration.User3, registration.User3, teamRegistration.TeamName) == -1)
                {
                    teamRegistration.User3 = registration.User3;
                    teamRegistration.AvailSpots--;
                    Users[2] = registration.User3;
                }
                else
                {
                    teamRegistration.User3 = "There is no user in database";
                }
            }
            if (teamRegistration.User4 != registration.User4)
            {
                if (AddOrDenyUser(teamRegistration.User4, registration.User4, teamRegistration.TeamName) == 1)
                {
                    teamRegistration.User4 = registration.User4;
                    teamRegistration.AvailSpots++;
                    Users[3] = "Removed";
                }
                else if (AddOrDenyUser(teamRegistration.User4, registration.User4, teamRegistration.TeamName) == -1)
                {
                    teamRegistration.User4 = registration.User4;
                    teamRegistration.AvailSpots--;
                    Users[3] = registration.User4;
                }
                else
                {
                    teamRegistration.User4 = "There is no user in database";
                }
            }
            if (teamRegistration.User5 != registration.User5)
            {
                if (AddOrDenyUser(teamRegistration.User5, registration.User5, teamRegistration.TeamName) == 1)
                {
                    teamRegistration.User5 = registration.User5;
                    teamRegistration.AvailSpots++;
                    Users[4] = "Removed";
                }
                else if (AddOrDenyUser(teamRegistration.User5, registration.User5, teamRegistration.TeamName) == -1)
                {
                    teamRegistration.User5 = registration.User5;
                    teamRegistration.AvailSpots--;
                    Users[4] = registration.User5;
                }
                else
                {
                    teamRegistration.User5 = "There is no user in database";
                }
            }
            if (teamRegistration.User6 != registration.User6)
            {
                if (AddOrDenyUser(teamRegistration.User6, registration.User6, teamRegistration.TeamName) == 1)
                {
                    teamRegistration.User6 = registration.User6;
                    teamRegistration.AvailSpots++;
                    Users[5] = "Removed";
                }
                else if (AddOrDenyUser(teamRegistration.User6, registration.User6, teamRegistration.TeamName) == -1)
                {
                    teamRegistration.User6 = registration.User6;
                    teamRegistration.AvailSpots--;
                    Users[5] = registration.User6;
                }
                else
                {
                    teamRegistration.User6 = "There is no user in database";
                }
            }
            return teamRegistration;
        }
        public static TeamRegistration GetJoinTeamRegistration(TeamRegistration teamRegistration, TeamRegistration registration, string userName)
        {
            if (registration.User1 == null)
            {
                registration.User1 = userName;
                registration.AvailSpots--;

            }
            else if (registration.User2 == null)
            {
                registration.User2 = userName;
                registration.AvailSpots--;
            }
            else if (registration.User3 == null)
            {
                registration.User3 = userName;
                registration.AvailSpots--;
            }
            else if (registration.User4 == null)
            {
                registration.User4 = userName;
                registration.AvailSpots--;
            }
            else if (registration.User5 == null)
            {
                registration.User5 = userName;
                registration.AvailSpots--;
            }
            else if (registration.User6 == null)
            {
                registration.User6 = userName;
                registration.AvailSpots--;
            }
            else
            {
                registration.TeamName = "Team already has 6 users";
            }
            return registration;
        }
    }
}

