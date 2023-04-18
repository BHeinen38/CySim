
using System;
using System.ComponentModel.DataAnnotations;

namespace CySim.Models.Scenario
{
    public class Scenario
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public bool isRed { get; set; }

        [Required]
        [MaxLength(50)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        [MaxLength(512)]
        public string FilePath { get; set; }
    }
}

