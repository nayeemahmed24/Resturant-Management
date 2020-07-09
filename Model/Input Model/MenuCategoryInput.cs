using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Input_Model
{
    public class MenuCategoryInput
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string CategoryTitle { get; set; }
        public string ParentId { get; set; }
        [Required]
        public String RestaurantId { get; set; }

    }
}
