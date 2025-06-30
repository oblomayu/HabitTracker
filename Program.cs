using Microsoft.EntityFrameworkCore;
using gut;
using gut.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/Login";
        options.Cookie.Name = "HabitTracker.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<GutDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("GutDbContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

// app.UseHttpsRedirection(); // Временно отключено для отладки

app.UseRouting();

// Добавляем логирование для отладки аутентификации
app.Use(async (context, next) =>
{
    var authCookie = context.Request.Cookies["HabitTracker.Auth"];
    Console.WriteLine($"Request to {context.Request.Path}: Auth cookie = {(authCookie != null ? "Present" : "Missing")}");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();