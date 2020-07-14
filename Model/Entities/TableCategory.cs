using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class TableCategory
    {
        [BsonId]
        public string Id { get; set; }
        public string ParentId { get; set; }
        [Required]
        public string CategoryTitle { get; set; }
        
        public string ResturantId { get; set; }
    }
}
