using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class Table
    {
        [BsonId]
        public string Id { get; set; }
        [Required]
        public int TableTitle { get; set; }
        [Required]
        public string TableCategoryId { get; set; }
        public bool available { get; set; }

    }
}
