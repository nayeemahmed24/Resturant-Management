using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model.Input_Model
{
    public class InvitationModel
    {
        [Required]
        public string restaurantName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required(ErrorMessage = "Email address is required")]
        public string email { get; set; }
    }
}
