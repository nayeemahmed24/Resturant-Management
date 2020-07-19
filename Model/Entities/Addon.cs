using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class Addon
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public string AddonTitle { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string ResturantId { get; set; }
        [Required]
        public string ParentId { get; set; }
        [DefaultValue(false)]
        public bool Available { get; set; }

    }
}
