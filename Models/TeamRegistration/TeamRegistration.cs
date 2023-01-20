using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CySim.Models.TeamRegistration
{
	public class TeamRegistration
	{
		public int Id { get; set; }

		public string TeamName { get; set; }

		public int AvailableSpots { get; set; }

		public int SpotsTaken { get; set; }

		public bool ProfilePicture { get; set; }

		public List<IdentityUser> Users { get; set; }

	}
}

