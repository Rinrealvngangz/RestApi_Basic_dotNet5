using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace RestAPI_Food.Etites
{
    public class Food
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Please enter valid decimal Number")]
        public decimal Price { get; set; }


        public Guid? CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
