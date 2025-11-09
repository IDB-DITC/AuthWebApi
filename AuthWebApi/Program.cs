
using AuthWebApi.Models;
using AuthWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace AuthWebApi
{
  public class Program
  {
    //public const string Key = "1234567812345678123456781234567812345678123456781234567812345678";

    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.

      builder.Services.AddControllers().AddJsonOptions(op =>
      {
        op.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        op.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        op.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

      });
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(options =>
      {

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
        {
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "Provide Token",
          Name = "Authorization",
          Scheme = "bearer",
          Type = SecuritySchemeType.Http
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
          {
            new OpenApiSecurityScheme()
            {
              Reference = new OpenApiReference()
              {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
              }
            },
             Array.Empty<string>()
          }
        });
      });



      builder.Services.AddDbContext<AppDbContext>(opt =>
      {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
      });

      builder.Services.AddIdentity<AppUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        ;

      builder.Services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme
          = options.DefaultScheme
          = options.DefaultChallengeScheme
          = options.DefaultForbidScheme
          = options.DefaultSignInScheme
          = options.DefaultSignOutScheme
          = JwtBearerDefaults.AuthenticationScheme;








        }).AddJwtBearer(bearer =>
        {


          byte[] key = Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key"));
          //byte[] key = Encoding.UTF8.GetBytes(Key);

          bearer.RequireHttpsMetadata = false;



          bearer.TokenValidationParameters = new TokenValidationParameters()
          {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)


          };

        });
      builder.Services.AddAuthorization(opt =>
      {
        opt.AddPolicy("Editor", policyOpt =>
      {
        //policyOpt.RequireUserName("admin");
        //policyOpt.RequireRole("Admin");
        //policyOpt.RequireRole("Moderator");

        policyOpt.RequireAssertion(ctx => ctx.User.Claims.Any(c=>( c.Type == ClaimTypes.Role && (c.Value=="Admin" || c.Value == "Moderator")) || (c.Type == ClaimTypes.Name && c.Value.ToLower().Contains("admin"))));


      });

      });



      builder.Services.AddScoped<IImageUpload, ImageUpload>();


      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/error");
      }

      app.UseHttpsRedirection();

      app.UseStaticFiles();


      ///app.UseRouting();
      app.UseAuthentication();


      app.UseAuthorization();


      app.MapControllers();

      app.Run();
    }
  }
}
