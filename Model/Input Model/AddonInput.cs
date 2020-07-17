using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Model.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Input_Model
{
    public class AddonInput
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string AddonTitle { get; set; }
        [Required]
        public double Price { get; set; }
        
        public string ResturantId { get; set; }
    }
}
