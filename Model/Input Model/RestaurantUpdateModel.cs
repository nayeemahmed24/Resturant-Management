using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Model.Validations;

namespace Model.Input_Model
{
    public class RestaurantUpdateModel
    {
        [StringLength(20, MinimumLength = 3)]
        public string restaurantName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string managerFirstName { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string managerLastName { get; set; }

        [MinLength(6)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[A-Z]+.*)(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$", ErrorMessage = "Password must contain Uppercase and lowercase letter, numbers and minimum length 6")]
        public string password { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string email { get; set; }

        public string token { get; set; }

        [FileSizeValidation(5)]
        [FileFormatValidation(".jpg|.png")]
        public IFormFile logo { get; set; }
    }
}
