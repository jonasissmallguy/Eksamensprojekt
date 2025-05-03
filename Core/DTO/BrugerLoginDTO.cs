using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Core
{

    public class BrugerLoginDTO
    {
        
        [BsonId]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Du skal udfylde en gyldig e-mail")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Du skal udfylde et password")]
        public string Password { get; set; }
        public string Rolle { get; set; }
        

    }

}