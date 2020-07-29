using System;
using System.Collections.Generic;
using System.Text;

namespace Model.View_Model
{
    public class ItemTypeAnalysis
    {
        public List<ItemUnit> ItemUnits { get; set; }
    }

    public class ItemUnit
    {
        public string ItemType { get; set; }
        public int Quantity { get; set; }
        public double TotalSell { get; set; }
    }
}
