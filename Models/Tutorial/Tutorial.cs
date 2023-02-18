using System;
using System.ComponentModel.DataAnnotations;

namespace CySim.Models.Tutorial
{
	public class Tutorial
	{
        [Required]
        public  int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string isRed { get; set; }

        [Required]
        [MaxLength(50)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        [MaxLength(512)]
        public string FilePath { get; set; }

        [Required]
        [MaxLength(10)]
        public string isGameType { get; set; }
        //if true then it will be a game tutorial
    }
}

