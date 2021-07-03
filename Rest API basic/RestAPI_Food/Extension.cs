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

        public static CategoryDtos AsCategoryDtos(this Category category)
        {
            if(category.Foods == null)
            {
                return new CategoryDtos()
                {
                    Id = category.CategoryId.ToString(),
                    Name = category.Name,
                    Foods = null

                };
            }
            var foods = category.Foods.Select(x => x.AsFoodDtos()).ToList();
            
            return new CategoryDtos()
            {
                Id = category.CategoryId.ToString(),
                Name = category.Name,
                Foods = new List<FoodDtos>(foods)
               
            };
        }
    }
}
