using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.Entities
{
    public class AddonUnit
    {
        [Required]
        public string AddonId { get; set; }
        [Required]
        public double Quantity { get; set; }
    }
}
