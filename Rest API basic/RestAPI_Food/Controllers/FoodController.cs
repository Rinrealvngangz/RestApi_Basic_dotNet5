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

namespace RestAPI_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                Status = StatusCodes.Status404NotFound,
                Success = false,
                Errors = new List<string>()
               {
                   $"Cannot find id {id} in Food"
               }
            });
        }



    }
}
