using AuthWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWebApi.Controllers
{
  [Route("[controller]/[action]")]
  [ApiController]
  public class TokenController(SignInManager<AppUser> signManager, UserManager<AppUser> userManager, IConfiguration configuration) : ControllerBase
  {
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserDto userDto)
    {
      try
      {

        var appUser = new AppUser
        {
          UserName = userDto.UserName,
          Email = userDto.UserName
        };
        var result = await userManager.CreateAsync(appUser, userDto.Password);

        if (result.Succeeded)
        {
          return Ok("Registration success");
        }
        else
        {
          return BadRequest(result.Errors);
        }
      }
      catch (Exception err)
      {

        return BadRequest(err);
      }

    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserDto userDto)
    {
      try
      {
        var user = await userManager.FindByEmailAsync(userDto.UserName);

        if (user == null)
        {
          return BadRequest("Bad credential");
        }

        var success = await userManager.CheckPasswordAsync(user, userDto.Password);
        if (success)
        {
          var claims = new List<Claim>();
          claims.Add(new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()));
          claims.Add(new Claim(ClaimTypes.Name, user.UserName));
          claims.Add(new Claim(ClaimTypes.Email, user.Email));

          //foreach (var role in await userManager.GetRolesAsync(user))
          //{
          //  claims.Add(new Claim(ClaimTypes.Role, role));
          //}

          claims.AddRange((await userManager.GetRolesAsync(user)).Select(role => new Claim(ClaimTypes.Role, role)));



          var keyBytes = Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key"));

          var securityKey = new SymmetricSecurityKey(keyBytes);
          var signCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
          var issuer = configuration.GetValue<string>("Jwt:Issuer");
          var jwtToken = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(7), signingCredentials: signCredential);

          var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

          return Ok(token);

        }
        else
        {
          return BadRequest("Bad credential");
        }


      }
      catch (Exception er)
      {
        return BadRequest(er);
      }
      return Ok(userDto);
    }
    [Authorize,HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
      string name = "", email = "";List<string> roles =[];
      try
      {
        var loggedinUser = HttpContext.User;


        if (loggedinUser != null)
        {
          name = loggedinUser.Identity.Name;

          foreach (var claim in loggedinUser.Claims)
          {
            if (claim.Type == ClaimTypes.Email)
            {
              email = claim.Value;
            }
            else if (claim.Type == ClaimTypes.Email)
            {
              roles.Add(claim.Value);
            }
            else if (claim.Type == ClaimTypes.Role)
            {
              roles.Add(claim.Value);
            }            
          }
        }
      }
      catch
      {

        return BadRequest();
      }
      return Ok(new { name, email, roles = string.Join(",", roles) });
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> GetUserClaims()
    {
      var loggedinUser = HttpContext.User;
      return Ok(new { Name = loggedinUser.Identity.Name, Claims = loggedinUser.Claims});
    }
  }
}
