using Microsoft.EntityFrameworkCore; // DbContext ve ilgili sýnýflarý kullanmak için
using Npgsql.EntityFrameworkCore.PostgreSQL; // PostgreSQL için EF Core saðlayýcýsýný kullanmak için
using KutuphaneOtomasyon.Web.Models; // Kendi DbContext'imizi kullanmak için (burasý doðru klasör yolu, çünkü ApplicationDbContext'i Models klasörüne koyduk)

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

// Buraya EKLÝYORUZ: Veritabaný baðlantýsýný ve DbContext'i servislere ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
