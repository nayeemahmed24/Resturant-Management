using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Entities
{
    public class ClientModel
    {
        [BsonId]
        public string _id { get; set; }
        public string host { get; set; }
        public string invitationRoute { get; set; }
    }
}
