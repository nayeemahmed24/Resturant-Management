using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class RestaurantModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string restaurantName { get; set; }
        public string managerFirstName { get; set; }
        public string managerLastName { get; set; }
        public string logo { get; set; }
        public string backgroundImage { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public bool isEmailVerified { get; set; }
        public bool isBlockedUser { get; set; }
        public string role { get; set; }
    }
}
