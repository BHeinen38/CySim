using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CySim.Models.TeamRegistration
{
	public class TeamRegistration
	{
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TeamName { get; set; }

		public int AvailableSpots { get; set; }

		public int SpotsTaken { get; set; }

		public bool ProfilePicture { get; set; }

		[NotMapped]
		public List <String> Users { get; set; }
		

	}
}

