using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.Input_Model
{
    public class AssignAddon
    {
        [Required]
        public string MenuItemId { get; set; }
        [Required]
        public string AddonId { get; set; }
    }
}
