using AuthWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthWebApi.Controllers
{
  [Route("[controller]/[action]")]
  [ApiController]
  public class ManageController(IConfiguration config, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) : ControllerBase
  {
    [ActionName("Users"), HttpGet]

    public async Task<IActionResult> GetUsers()
    {
      var data = await userManager.Users.ToListAsync();
      return Ok( data.Select(u=> new { u.Id, u.UserName, u.Email, u.PhotoPath, u.PhoneNumber}));
    }

   
    [ActionName("Roles"), HttpGet]

    public async Task<IActionResult> GetRoles()
    {
      var data = await roleManager.Roles.ToListAsync();

      return Ok(data.Select(r=> new { r.Id, r.Name }));
    }


    [ActionName("Roles"), HttpGet("{userName}")]

    public async Task<IActionResult> GetRolesByUser(string userName)
    {
      var user = await userManager.FindByNameAsync(userName);

      if (user is null)
      {
        return BadRequest("invalid user");
      }
      var roles = await userManager.GetRolesAsync(user);

      return Ok(roles);
    }

    [ActionName("Users"), HttpGet("{roleName}")]

    public async Task<IActionResult> GetUsersByRole(string roleName)
    {
      var users = await userManager.GetUsersInRoleAsync(roleName);

      return Ok(users.Select(u => new { u.Id, u.UserName, u.Email, u.PhotoPath, u.PhoneNumber }));
    }



    [ActionName("SaveRole"), HttpPost]

    public async Task<IActionResult> CreateRoles(string roleName)
    {
      var result = await roleManager.CreateAsync(new IdentityRole(roleName));
      if (result.Succeeded)
      {
        return Ok($"{roleName} role created successfully...");
      }
      return BadRequest(result.Errors);
    }


    [HttpPost]

    public async Task<IActionResult> AssignRole(UserRoleAssignDto data)
    {
      try
      {

        var user = await userManager.FindByNameAsync(data.UserName);

        if(user is null)
        {
          return BadRequest("invalid user");
        }

        var oldRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, oldRoles);
        await userManager.AddToRolesAsync(user, data.Roles);


      }
      catch (Exception err)
      {
        return BadRequest(err);
      }
      return Ok(data);
    }


  }
}
