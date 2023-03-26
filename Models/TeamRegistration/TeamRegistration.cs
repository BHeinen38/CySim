using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Display(Name ="Team Name")]
        public string TeamName { get; set; }

        [Display(Name ="Spots Taken")]
        public int SpotsTaken { get; set; }

        public bool ProfilePicture { get; set; }

        [MaxLength(50)]
        public string User1 { get; set; }

        [MaxLength(50)]
        public string User2 { get; set; }

        [MaxLength(50)]
        public string User3 { get; set; }

        [MaxLength(50)]
        public string User4 { get; set; }

        [MaxLength(50)]
        public string User5 { get; set; }

        [MaxLength(50)]
        public string User6 { get; set; }

        [DefaultValue(0)]
        public int Usability { get; set; }

        [DefaultValue(0)]
        public int Score { get; set; }

        [DefaultValue(0)]
        public int Flags { get; set; }
    }
}
