using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

//This might be useful if there is ever a severe problem with Identity.
//This could help get you started

namespace CySim.Models
{
	public class ApplicationUser : IdentityUser
	{
		[NotMapped]
		public string Role { get; set; }
	}
}

