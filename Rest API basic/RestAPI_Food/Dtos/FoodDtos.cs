using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI_Food.Dtos
{
    public class FoodDtos
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
