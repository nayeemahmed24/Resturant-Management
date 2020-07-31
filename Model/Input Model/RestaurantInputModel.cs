using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Model.Validations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Input_Model
{
    public class RestaurantInputModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(20, MinimumLength = 3)]
        public string restaurantName { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(20, MinimumLength = 3)]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(20, MinimumLength = 3)]
        public string lastName { get; set; }

        [Required(ErrorMessage ="User name is required")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[A-Z]+.*)(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$", ErrorMessage = "Password must contain Uppercase and lowercase letter, numbers and minimum length 6")]
        public string password { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required(ErrorMessage = "Email address is required")]
        public string email { get; set; }

        [FileSizeValidation(5, ErrorMessage = "File size can't be larger than 5 MB")]
        [FileFormatValidation(".jpg|.png", ErrorMessage = "Only '.jpg' & '.png' files are supported")]
        public IFormFile logo { get; set; }

        [FileSizeValidation(10, ErrorMessage = "File size can't be larger than 10 MB")]
        [FileFormatValidation(".jpg|.png", ErrorMessage = "Only '.jpg' & '.png' files are supported")]
        public IFormFile backgroundImage { get; set; }

        public string invitationToken { get; set; }
    }
}
