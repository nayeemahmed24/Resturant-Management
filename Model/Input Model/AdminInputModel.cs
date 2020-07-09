using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Input_Model
{
    public class AdminInputModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(6)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[A-Z]+.*)(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,}$", ErrorMessage = "Password must contain Uppercase and lowercase letter, numbers and minimum length 6")]

        public string password { get; set; }


        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; set; }
        public string role { get; set; }

    }
}
