using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FlintSoftFood.Contracts.Models
{
    public class Food
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int EAN { get; set; }

        [Required]
        public int Calories { get; set; }

        [Required]
        public decimal Carbohydrates { get; set; }

        public decimal Fat { get; set; }
    }
}
