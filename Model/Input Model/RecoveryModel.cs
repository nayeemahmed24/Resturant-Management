using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Model.Input_Model
{
    public class RecoveryModel
    {
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        public string username { get; set; }

        [MinLength(6)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[A-Z]+.*)(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$", ErrorMessage = "Password must contain Uppercase and lowercase letter, numbers and minimum length 6")]
        public string password { get; set; }

        public string token { get; set; }
        public string clientId { get; set; }
    }
}
