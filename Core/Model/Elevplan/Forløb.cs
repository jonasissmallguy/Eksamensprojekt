﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core
{

    public class Forløb
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Semester { get; set; }
        public List<Goal> Goals { get; set; } 
        public string Status { get; set; }
    }
}