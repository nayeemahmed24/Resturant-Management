using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class AddonCategory
    {
        [BsonId]
        public string Id { get; set; }
        [Required]
        public string AddonCategoryTitle { get; set; }
        public string ParentId { get; set; }
      
        public string RestaurantId { get; set; }
       
    }
}
