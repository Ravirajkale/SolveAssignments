using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using BasicEF.Data_Access_Layer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using System.Text.Json.Serialization;

namespace BasicEF.Entites
{
    [Index(nameof(email), IsUnique = true)]
    public class student
    {
        [Key]
        [JsonIgnore]
        public int id { get; set; }

        public string name { get; set; }

        
        public string email { get; set; }

        public string password { get; set; }
    }



}
