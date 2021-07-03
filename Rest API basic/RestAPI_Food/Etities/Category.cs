using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace RestAPI_Food.Etites
{
    
    public class Category
    {
        
        [Required]
        public Guid CategoryId { get; set; }
      
        public string Name { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual  ICollection<Food> Foods { get; set; }

    }
}
