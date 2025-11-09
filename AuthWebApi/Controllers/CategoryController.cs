using AuthWebApi.Models;
using AuthWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class CategoryController(AppDbContext db) : ControllerBase
  {
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
      return Ok(await db.Categories.Include(c => c.Products).ToListAsync());
    }


    [HttpGet("{catId}")]
    public async Task<IActionResult> GetById(int catId)
    {

      var category = await db.Categories.FindAsync(catId);

      if (category is null)
      {
        return NotFound($"no category found by id {catId}");
      }
      await db.Entry(category).Collection<Product>(c => c.Products).LoadAsync();

      return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Save(Category category)
    {
      try
      {
        //category.Create(HttpContext.User.Identity.Name);

        db.Categories.Add(category);

        await db.SaveChangesAsync();
        return Ok(category);
      }
      catch
      {
        return BadRequest(ModelState);
      }

    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category, CancellationToken C)
    {
      if (id != category.Id)
      {
        return BadRequest("id invalid");
      }

      try
      {
        var old = await db.Categories.Include(c => c.Products).FirstAsync(c => c.Id == id);

        if (old is not null)
        {
          db.Categories.Remove(old);
        }

        //category.Update(HttpContext.User.Identity.Name);


        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync(C);
        return Ok(category);
      }
      catch
      {
        return BadRequest(ModelState);
      }

    }

    [HttpPost("Upload")]
    public async Task<IActionResult> Upload([FromServices]IImageUpload upload, IFormFile file, CancellationToken C)
    {
      return Ok(await upload.UploadFile(file, C));
    }

  }
}
