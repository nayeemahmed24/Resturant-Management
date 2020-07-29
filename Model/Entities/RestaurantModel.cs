using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Model.Entities
{
    public class RestaurantModel
    {
        public string Id { get; set; }
        public string restaurantName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public string username { get; set; }
        public string logo { get; set; }
        public string backgroundImage { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        //public bool isEmailVerified { get; set; }
        public bool isBlockedUser { get; set; }
        [DefaultValue(ResturantStatus.Close)]
        public ResturantStatus OpenClose { get; set; }
        public string role { get; set; }
    }
}
