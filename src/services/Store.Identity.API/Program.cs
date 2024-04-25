using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Store.Identity.API.Data;
using Store.Identity.API.Extensions;
using Store.Identity.API.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ISignInService, SignInService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Store Enterprise",
            Description = "Store Enterprise API",
            Version = "v1",
            Contact = new OpenApiContact()
            {
                Name = "Heng Tan",
                Url = new Uri("https://github.com/hengtan"),
            },
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };
//
// app.MapGet("/weatherforecast", () =>
//     {
//         var forecast = Enumerable.Range(1, 5).Select(index =>
//                 new WeatherForecast
//                 (
//                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                     Random.Shared.Next(-20, 55),
//                     summaries[Random.Shared.Next(summaries.Length)]
//                 ))
//             .ToArray();
//         return forecast;
//     })
//     .WithName("GetWeatherForecast")
//     .WithOpenApi();
//
// app.Run();
//
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

app.MapPost("/api/identity/new-account", async (UserViewModels.UserRegister userRegister, HttpContext httpContext) =>
{
    var signInService = httpContext.RequestServices.GetRequiredService<ISignInService>();
    var userService = httpContext.RequestServices.GetRequiredService<IUserService>();

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(userRegister);

    if (!Validator.TryValidateObject(userRegister, validationContext, validationResults, true))
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(validationResults);
        return;
    }

    var user = new IdentityUser
    {
        UserName = userRegister.Email,
        Email = userRegister.Email,
        EmailConfirmed = true
    };

    var result = await userService.UserManager.CreateAsync(user, userRegister.Password);

    if (result.Succeeded)
    {
        await signInService.SignInManager.SignInAsync(user, false);
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        await httpContext.Response.WriteAsJsonAsync(user);
    }
    else
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(result.Errors);
    }
});

// [HttpPost("/new-account")]
app.MapPost("/api/identity/authentication", async (UserViewModels.UserLogin userLogin, HttpContext httpContext) =>
{
    var signInService = httpContext.RequestServices.GetRequiredService<ISignInService>();
    var userService = httpContext.RequestServices.GetRequiredService<IUserService>();

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(userLogin);

    if (!Validator.TryValidateObject(userLogin, validationContext, validationResults, true))
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(validationResults);
        return;
    }

    var result = await signInService.SignInManager.PasswordSignInAsync(userLogin.Email,
        userLogin.Password, false, true);

    if (result.Succeeded)
    {
        httpContext.Response.StatusCode = StatusCodes.Status200OK;
        return;
    }
    else
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }
});

app.Run();