using System;
using System.Collections.Generic;
using System.Text;
using Model.Entities;

namespace Model.View_Model
{
    public class MenuItemDetailes
    {
        public MenuItem Menu { get; set; }
        public List<Addon> Addons { get; set; }
    }
}
