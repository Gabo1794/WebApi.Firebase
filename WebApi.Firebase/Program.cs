//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;

using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Firebase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, 
    FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme,
    options => { });


    //.AddJwtBearer(options =>
    //{
    //    options.Audience = "https://securetoken.google.com/1:804749815759:web:7bd39f61eceaab7b8aecba";
    //    options.TokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = true,
    //        ValidIssuer = "https://securetoken.google.com/1:804749815759:web:7bd39f61eceaab7b8aecba",
    //        ValidateAudience = true,
    //        ValidAudience = "1:804749815759:web:7bd39f61eceaab7b8aecba",
    //        ValidateLifetime = true
    //    };
    //});

//Add sessions
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30);//We set Time here 
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();

app.MapControllers();

app.Run();
