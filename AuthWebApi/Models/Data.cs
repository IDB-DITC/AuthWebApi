using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthWebApi.Models
{

  //public abstract class BaseEntity
  //{

  //  public DateTime CreatedAt { get; set; } = DateTime.Now;
  //  public string? CreatedBy { get; set; }
  //  public DateTime? ModifiedAt { get; set; }
  //  public string? ModifiedBy { get; set; }
  //  public DateTime? DeletedAt { get; set; } 
  //  public string? DeletedBy { get; set; }


  //  public virtual void Create(string userid)
  //  {
  //    CreatedAt = DateTime.Now;
  //    CreatedBy = userid;
  //  }
  //  public virtual void Update(string userid)
  //  {
  //    ModifiedAt = DateTime.Now;
  //    ModifiedBy = userid;
  //  }
  //  public virtual void Delete(string userid)
  //  {
  //    DeletedAt = DateTime.Now;
  //    DeletedBy = userid;
  //  }
  //}



  public sealed class Category
  {
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false), StringLength(30, MinimumLength = 2)]

    public string Name { get; set; } = default!;

    [DataType(DataType.ImageUrl)]
    [Column(TypeName = "varchar")]
    public string? ImageUrl { get; set; }

    public IList<Product> Products { get; set; } = [];

  }

  public sealed class Product
  {
    [Required]
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Name { get; set; } = default!;

    [Range(1, int.MaxValue)]
    public decimal Price { get; set; }

    [ForeignKey("Category")]
    public int CatId { get; set; }
    public Category? Category { get; set; }


  }

  public class AppUser:IdentityUser
  {
    [DataType(DataType.ImageUrl)]
    public string? PhotoPath { get; set; }
  }



  public class AppDbContext:IdentityDbContext<AppUser>
  {
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions o):base(o)
    {
      
    }
  }

}
