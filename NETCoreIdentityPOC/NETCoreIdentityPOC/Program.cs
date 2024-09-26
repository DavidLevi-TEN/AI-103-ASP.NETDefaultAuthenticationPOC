using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCoreIdentityPOC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddAuthorization(); // < Add services to DI container and enable Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>() // Activate < the Identity API
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cookies are used by default

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>(); // Map the routes for Identity API

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
