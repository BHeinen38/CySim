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
        public static int AddOrDenyUser(string oldUser, string newUser)
        {
            int userInt = -1;
            var user = FindUser(newUser);
            if (newUser == null)
            {
                userInt = 1;
                return userInt;

            }
            else
            {
                if (user == null)
                {
                    userInt = 0;
                    return userInt;
                }
                else
                {
                    userInt = -1;
                    return userInt;
                }
            }
        }

        public static TeamRegistration GetEditTeamRegistration(TeamRegistration registration, TeamRegistration teamRegistration, out string[] Users)
        {
            Users = new string[6];
            teamRegistration.TeamName = registration.TeamName;

            string[] regUsers = new string[6] { registration.User1, registration.User2, registration.User3, registration.User4,
            registration.User5, registration.User6};

            string[] teamRegUsers = new string[6] { teamRegistration.User1, teamRegistration.User2, teamRegistration.User3, teamRegistration.User4,
            teamRegistration.User5, teamRegistration.User6};

            for(int i = 0; i < regUsers.Length; i++)
            {
                if (teamRegUsers[i] != regUsers[i])
                {
                    if (AddOrDenyUser(teamRegUsers[i], regUsers[i]) == 1)
                    {
                        switch (i)
                        {
                            case 0:
                                teamRegistration.User1 = registration.User1;
                                break;
                            case 1:
                                teamRegistration.User2 = registration.User2;
                                break;
                            case 2:
                                teamRegistration.User3 = registration.User3;
                                break;
                            case 3:
                                teamRegistration.User4 = registration.User4;
                                break;
                            case 4:
                                teamRegistration.User5 = registration.User5;
                                break;
                            case 5:
                                teamRegistration.User6 = registration.User6;
                                break;
                        }
                        teamRegistration.AvailSpots++;
                        Users[i] = "Removed";

                    }
                    else if (AddOrDenyUser(teamRegUsers[i], regUsers[i]) == -1)
                    {
                        switch (i)
                        {
                            case 0:
                                teamRegistration.User1 = registration.User1;
                                break;
                            case 1:
                                teamRegistration.User2 = registration.User2;
                                break;
                            case 2:
                                teamRegistration.User3 = registration.User3;
                                break;
                            case 3:
                                teamRegistration.User4 = registration.User4;
                                break;
                            case 4:
                                teamRegistration.User5 = registration.User5;
                                break;
                            case 5:
                                teamRegistration.User6 = registration.User6;
                                break;
                        }
                        teamRegistration.AvailSpots--;
                        Users[i] = regUsers[i];
                    }
                    else
                    {
                        Users[i] = "There is no user in database";
                    }
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

