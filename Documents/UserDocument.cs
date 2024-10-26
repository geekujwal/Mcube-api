using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Contracts;

namespace Documents
{
    public class UserDocument
    {
        public string Id { get; set; }

        private string _email;

        public string FullName {get; set;}

        public string PhoneNumber {get; set;}

        public string Email
        {
            get => _email;
            set => _email = value?.ToLowerInvariant().Trim();
        }

        public string Hash { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public AccountType AccountType { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsBlocked { get; set; } = false;
    }
}