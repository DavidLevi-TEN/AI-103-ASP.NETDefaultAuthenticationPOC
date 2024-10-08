using ASP.NETDefaultAuthenticationPOC.Authorization;
using ASP.NETDefaultAuthenticationPOC.Authorization.AgeRequirement;
using ASP.NETDefaultAuthenticationPOC.Data;
using ASP.NETDefaultAuthenticationPOC.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeAuthorizationHandler>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.EventsType = typeof(CustomCookieAuthentication);
    })
    .AddJwtBearer(options =>
    {
        options.Audience = builder.Configuration["AzureAd:ClientId"];
        options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}";
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

// Authentication and Authorization handlers.
builder.Services.AddScoped<CustomCookieAuthentication>();
builder.Services.AddScoped<IAuthorizationHandler, OwnerAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ManagerAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AdminstratorAuthorizationHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    // requires using Microsoft.Extensions.Configuration;
    // Set password with the Secret Manager tool.
    // dotnet user-secrets set SeedUserPW <pw>

    var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW")!;

    await SeedData.Initialize(services, testUserPw);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax // The default value is Lax
    // SameSiteMode.Lax allows for cross-origin authentication such as OAuth2
    // SameSiteMode.Strict breaks OAuth2 but elevates security for apps without cross-origin authentication
};

app.UseCookiePolicy(cookiePolicyOptions);

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
