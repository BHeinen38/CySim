using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace CySim.Models
{
	public class ApplicationUser : IdentityUser
	{
		[NotMapped]
		public string Role { get; set; }
	}
}

