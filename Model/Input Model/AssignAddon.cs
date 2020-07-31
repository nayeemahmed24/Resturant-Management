using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.Input_Model
{
    public class AssignAddon
    {
        [Required]
        public string ItemId { get; set; }
        [Required]
        public string AddonCategoryId { get; set; }
    }
}
