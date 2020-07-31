using System;
using System.Collections.Generic;
using System.Text;
using Model.Entities;

namespace Model.View_Model
{
    public class SoldQuantity
    {
        public string ItemType { get; set; }
        public int Quantity { get; set; }
 
        public double TotalPrice { get; set; }
    }
}
