using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using RestAPI_Food.Etites;

namespace RestAPI_Food.Dtos
{
    public class CategoryDtos
    {
        [Required]
        public string  Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public List<FoodDtos> Foods { get; set; }

    }
}
