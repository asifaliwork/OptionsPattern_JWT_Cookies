using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using OptionsPattern.Data;
using OptionsPattern.Models.Account;
using OptionsPattern.Models.AppSettings;
using OptionsPattern.Services.Account;
using OptionsPattern.Services.Items;
using OptionsPattern.Services.Seed;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 2;

}).AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddOptions<AppSettings>().BindConfiguration(nameof(AppSettings))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<IDbInitSeeding , DbInitSeeding>();
builder.Services.AddScoped<IAccountServices , AccountServices>();
builder.Services.AddScoped<IItemServices , ItemServices>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "SecondToken";
    options.DefaultChallengeScheme = "SecondToken";
   
    options.DefaultAuthenticateScheme = "SecondToken";
})
 .AddJwtBearer(options =>
 {
      
       options.TokenValidationParameters = new TokenValidationParameters()
       {
           ValidateIssuer = true,
           ValidateActor = false,
           ValidateLifetime = true,
           ValidateAudience = true,
           ValidAudience = configuration["JWT:ValidAudience"],
           ValidIssuer = configuration["JWT:ValidIssuer"],
           ClockSkew = TimeSpan.Zero,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]!))
       };
 })
       .AddJwtBearer("SecondJwtToken", options =>
       {
           
           options.TokenValidationParameters = new TokenValidationParameters()
           {
               ValidateIssuer = true,
               ValidateActor = false,
               ValidateLifetime = true,
               ValidateAudience = true,
             
               ValidAudience = configuration["AppSettings:ValidAudience"],
               ValidIssuer = configuration["AppSettings:ValidIssuer"],
               ClockSkew = TimeSpan.Zero,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Key"]!))
           };
       })
   .AddCookie(options =>
   {
       
       options.SlidingExpiration = true;
       options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
       options.Events.OnRedirectToLogin = (context) =>
       {
           context.Response.StatusCode = 401;
           return Task.CompletedTask;
       };

   })
   .AddPolicyScheme("SecondToken", "SecondToken", options =>
   {
       options.ForwardDefaultSelector = context =>
       {
           string authorization = context.Request.Headers[HeaderNames.Authorization];
           if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
           {
               var token = authorization.Substring("Bearer ".Length).Trim();
               var jwtHandler = new JwtSecurityTokenHandler();


               return (jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Equals("JWT:ValidIssuer"))
                 ? JwtBearerDefaults.AuthenticationScheme : "SecondJwtToken";

           }

           // We don't know what it is
           return CookieAuthenticationDefaults.AuthenticationScheme;
       };
   });

builder.Services.AddAuthorization(options =>
{
    var DefaultPolicy = new AuthorizationPolicyBuilder
    (
        
       // JwtBearerDefaults.AuthenticationScheme,
        CookieAuthenticationDefaults.AuthenticationScheme,
        "SecondJwtToken"

    );
    options.DefaultPolicy = DefaultPolicy
          .RequireAuthenticatedUser()
          .Build();

    options.AddPolicy("SecondJwt", policy =>
    {
        

        policy.AddAuthenticationSchemes("SecondJwtToken");
        //new AuthorizationPolicyBuilder("SecondJwtToken");
        policy.RequireAuthenticatedUser()
        .Build();


    });

    options.AddPolicy("Jwt", policy =>
    {
        new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
        //new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser().Build();
    });


    options.AddPolicy("Cookies", policy =>
    {
        new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser().Build();
       
    });

    options.AddPolicy("JWTOrCookies", policy =>
    {
        new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
        new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser().Build();
    });


});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OptionsPattern", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please Enter Token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id ="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
//app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IDbInitSeeding>();
        context.Initialize();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


app.MapControllers();

app.Run();
