using RestAPI_Food.Dtos;
using RestAPI_Food.Etites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI_Food
{
    public static class Extension
    {
        public static FoodDtos AsFoodDtos (this Food food)
        {
            return new FoodDtos
            {

                Id = food.Id.ToString(),
                Name = food.Name,
                Price = food.Price,
                CategoryId = food.CategoryId
            };
        }
    }
}
