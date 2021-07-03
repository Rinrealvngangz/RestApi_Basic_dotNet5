using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI_Food.Dtos
{
    public class CategoryFoodDtos
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<FoodDtos> foods { get; set;}
    }
}
