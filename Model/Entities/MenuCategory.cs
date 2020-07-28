using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class MenuCatergory
    {
        [BsonId]
        public string Id { get; set; }
        [Required]
        public string CategoryTitle { get; set; }
        public string Parent { get; set; }
        [Required]
        public RestaurantModel Restaurant { get; set; }
        public List<string> AddonCategories { get; set; }
        [DefaultValue(false)]
        public bool IsChildAvailable { get; set; }
        [DefaultValue(false)]
        public bool ItemAdded { get; set; }
    }
}
