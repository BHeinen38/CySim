using System;
using System.ComponentModel.DataAnnotations;

namespace CySim.Models.ScoreBoardModels
{
	public class ScoreBoard
	{
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TeamName { get; set; }

		public int Score { get; set; }

		public bool ProfilePicutre { get; set; }

        public double Usability { get; set; }

        public double Flags { get; set; }
    }
}

