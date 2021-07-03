using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestAPI_Food.Data;
using Microsoft.EntityFrameworkCore;
using RestAPI_Food.Etites;
using System.Text.Json;
using RestAPI_Food.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace RestAPI_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        FoodDBContext _dBContext;
        public CategoryController(FoodDBContext dBContext)
        {
            _dBContext = dBContext;
        }
        [HttpGet]
        public async Task<ActionResult> GetCategorysAsync() 
        {
              var items = await _dBContext.Categories.Include(x => x.Foods).ToListAsync();       
            return Ok(JsonSerializer.Serialize(items.AsCategoryFoodDtos()));
        }
        [HttpGet("{id}")]

        public async Task<ActionResult> GetCategoryById(string id)
        {
           var existItem = await _dBContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == Guid.Parse(id));
           if(existItem == null)
            {
                return NotFound();
            }
            return Ok(existItem);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] Category category)
        {
            var item = new Category()
            {
                CategoryId = Guid.NewGuid(),
                Name = category.Name

            };
            await _dBContext.Categories.AddAsync(item);
            await _dBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = item.CategoryId }, item.AsCategoryDtos());
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id ,[FromBody] Category category)
        {
            var existItem = await _dBContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == Guid.Parse(id));
            if (existItem == null)
            {
                return NotFound();
            }
            existItem.Name = category.Name;
              _dBContext.Categories.Update(existItem);
            _dBContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> Delete(string id)
        {
            var existItem = await _dBContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == Guid.Parse(id));
            if (existItem == null)
            {
                return NotFound();
            }
       
            _dBContext.Categories.Remove(existItem);
            _dBContext.SaveChanges();
            return NoContent();
        }

    }
}
