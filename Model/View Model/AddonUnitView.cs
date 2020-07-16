using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Model.Entities;

namespace Model.View_Model
{
    public class AddonUnitView
    {
        [Required]
        public Addon Addon { get; set; }
        [Required]
        public double Quantity { get; set; }
    }
}
