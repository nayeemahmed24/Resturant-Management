using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.Entities
{
    public class AddonUnit
    {
        [Required]
        public string Id { get; set; }
        public string name { get; set; }
        public double Quantity { get; set; }
    }
}
