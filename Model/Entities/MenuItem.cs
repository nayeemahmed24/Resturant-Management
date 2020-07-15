using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class MenuItem
    {
        [BsonId]
        
        public string Id { get; set; }
        [Required]
        public string ItemTitle { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string ParentId { get; set; }

        public string description { get; set; }
        public string ItemType { get; set; }

        public List<string> AddonsList { get; set; }

        [DefaultValue(false)]
        public bool Available { get; set; }
        [Required]
        public RestaurantModel Restaurant { get; set; }

    }
}
