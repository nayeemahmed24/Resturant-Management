using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.Entities
{
    public class OrderUnit
    {
        [Required]
        public string MenuItemId { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public List<AddonUnit> Addons { get; set; }

    }
}
