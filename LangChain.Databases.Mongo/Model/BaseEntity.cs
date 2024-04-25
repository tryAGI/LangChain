using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LangChain.Databases.Mongo.Model
{
    public class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
