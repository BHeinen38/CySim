using System;
using System.ComponentModel.DataAnnotations;

namespace CySim.Models.Tutorial
{
    public class Tutorial
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool isRed { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        [MaxLength(512)]
        public string FilePath { get; set; }

        [Required]
        public bool isGameType { get; set; }
        //if true then it will be a game tutorial
    }
}

