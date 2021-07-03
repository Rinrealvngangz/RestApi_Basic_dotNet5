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
        public static IEnumerable<CategoryFoodDtos> AsCategoryFoodDtos(this IEnumerable<Category> items)
        {
            List<CategoryFoodDtos> ListCategoryFoods = new List<CategoryFoodDtos>();
            foreach (var cf in items)
            {
                List<FoodDtos> foodDtos = new List<FoodDtos>();
                cf.Foods.ToList().ForEach(x => foodDtos.Add(x.AsFoodDtos()));
                var categoryFoodDtos = new CategoryFoodDtos()
                {
                    Id = cf.CategoryId.ToString(),
                    Name = cf.Name,
                    foods = new List<FoodDtos>(foodDtos)

                };
                ListCategoryFoods.Add(categoryFoodDtos);
            }
            return ListCategoryFoods;
        }
    }
}
