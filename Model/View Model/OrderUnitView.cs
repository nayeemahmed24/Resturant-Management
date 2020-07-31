using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Model.Entities;

namespace Model.View_Model
{
    public class OrderUnitView
    {
        [Required]
        public MenuItem MenuItem { get; set; }
        public List<AddonUnitView> Addons { get; set; }
    }
}
