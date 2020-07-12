using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entities
{
    public class OrderUnit
    {
        public string MenuItemId { get; set; }
        public List<AddonUnit> Addons { get; set; }

    }
}
