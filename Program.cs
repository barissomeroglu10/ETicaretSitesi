using ETicaretSitesi.Data;
using ETicaretSitesi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ETicaretDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ML.NET Recommendation Service
builder.Services.AddScoped<IRecommendationService, RecommendationService>();

// Add Authentication - İki farklı scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Kullanici/Giris";
        options.LogoutPath = "/Kullanici/Cikis";
        options.AccessDeniedPath = "/Kullanici/Giris";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.Name = "UserAuth";
    })
    .AddCookie("AdminScheme", options =>
    {
        options.LoginPath = "/Kullanici/AdminGiris";
        options.LogoutPath = "/Kullanici/AdminCikis";
        options.AccessDeniedPath = "/Kullanici/AdminGiris";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "AdminAuth";
    });

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Data ve Rol Düzeltme
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ETicaretDbContext>();
        
        // Mevcut kullanıcıların rollerini düzelt
        var kullanicilar = context.Kullanicilar.ToList();
        bool rolGuncellendi = false;
        
        foreach (var kullanici in kullanicilar)
        {
            if (string.IsNullOrEmpty(kullanici.Rol) || 
                (kullanici.AdminMi && kullanici.Rol != "Admin") ||
                (!kullanici.AdminMi && kullanici.Rol != "Kullanici"))
            {
                kullanici.Rol = kullanici.AdminMi ? "Admin" : "Kullanici";
                rolGuncellendi = true;
            }
        }
        
        if (rolGuncellendi)
        {
            context.SaveChanges();
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Kullanıcı rolleri başarıyla güncellendi.");
        }
        
        // Eğer hiç kullanıcı yoksa seed data çalıştır
        if (!context.Kullanicilar.Any())
        {
            DbInitializer.Initialize(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database or fixing roles.");
    }
}

app.Run(); 