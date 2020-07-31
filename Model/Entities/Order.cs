using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [Required]
        public List<OrderUnit> Items { get; set; }
        public OrderStatus Status { get; set; }
        [Required]
        public double Amount { get; set; }
        [DefaultValue(false)]
        public bool Paid { get; set; }
        [Required]
        public Table TableNumber { get; set; }
        [Required]
        public string ResturantId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime OrderdAt { get; set; } = DateTime.Now;
    }
}
