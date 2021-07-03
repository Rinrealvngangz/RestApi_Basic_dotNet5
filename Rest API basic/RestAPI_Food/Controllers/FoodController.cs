using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestAPI_Food.Data;
using Microsoft.EntityFrameworkCore;
using RestAPI_Food.Etites;
using RestAPI_Food.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace RestAPI_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodController : ControllerBase
    {
        private readonly FoodDBContext _dbContext;
        public FoodController(FoodDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]

        public async Task<IActionResult> GetFood()
        {
            var items = await _dbContext.Foods.Select(x => x.AsFoodDtos()).ToListAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(Food itemsFood)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult("Something went wrong") { StatusCode = 500 };
            }

            var item = new Food
            {
                Id = new Guid(),
                Name = itemsFood.Name,
                Price = itemsFood.Price,
                Category = itemsFood.Category

            };
            await _dbContext.Foods.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(FindFoodById), new { id = item.Id }, item.AsFoodDtos());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> FindFoodById( string id)
        {
            var item = await _dbContext.Foods.FirstOrDefaultAsync(x => x.Id.ToString() == id);
            if(item != null)
            {

                return Ok(item.AsFoodDtos());
            }
           
            return NotFound(new ResponseDtos
            {
               
                Success = false,
                Errors = new List<string>()
               {
                   $"Cannot find id {id} in Food"
               }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existItem =  await _dbContext.Foods.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));
              if(existItem == null)
            {
                return BadRequest(new ResponseDtos()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        $"Cannot find {id} in Food"
                    }
                });
            }

            _dbContext.Foods.Remove(existItem);
            _dbContext.SaveChanges();
            return NoContent();

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id ,[FromBody] Food food)
        {
           var existItem = await _dbContext.Foods.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));
            if(existItem == null)
            {
                return BadRequest(new ResponseDtos()
                {
                    Success = false,
                    Errors = new List<string>()
                    {
                        $"Cannot find {id} in Food"
                    }
                });
            }
            existItem.Name = food.Name;
            existItem.Price = food.Price;
            existItem.CategoryId = food.CategoryId;
            _dbContext.Foods.Update(existItem);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        [Route("{id}/Category/{categoryId}")]
        public async Task<ActionResult> AddCategory(string id ,string categoryId)
        {
            var existCategory = await _dbContext.Categories.FindAsync(Guid.Parse(categoryId));
            var existFood = await _dbContext.Foods.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));
            if (existCategory == null || existFood == null)
            {
                return NotFound();
            }
            existFood.CategoryId = existCategory.CategoryId;
           
            _dbContext.Foods.Update(existFood);
            await _dbContext.SaveChangesAsync();
            return NoContent();
         
        }

        [HttpGet]
        [Route("{id}/Category")]
        public async Task<ActionResult> GetCategory(string id)
        {
            var existFood = await _dbContext.Foods.FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));
    
            if (existFood == null)
            {
                return NotFound();
            }
          var category =  await _dbContext.Categories.FirstOrDefaultAsync(x => x.Foods.Contains(existFood));
       
            return Ok(category.AsCategoryDtos());

        }

    }
}
