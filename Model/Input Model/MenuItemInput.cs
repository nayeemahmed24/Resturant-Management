using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Input_Model
{
    public class MenuItemInput
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string ItemTitle { get; set; }

        [Required]
        public double Price { get; set; }
        public string description { get; set; }
        public string ItemType { get; set; }
        [Required]
        public String ParentId { get; set; }
        public String ResturantId { get; set; }

    }
}
