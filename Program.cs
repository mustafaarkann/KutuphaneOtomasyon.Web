using Microsoft.EntityFrameworkCore; // DbContext ve ilgili s�n�flar� kullanmak i�in
using Npgsql.EntityFrameworkCore.PostgreSQL; // PostgreSQL i�in EF Core sa�lay�c�s�n� kullanmak i�in
using KutuphaneOtomasyon.Web.Models; // Kendi DbContext'imizi kullanmak i�in (buras� do�ru klas�r yolu, ��nk� ApplicationDbContext'i Models klas�r�ne koyduk)

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();

// Buraya EKL�YORUZ: Veritaban� ba�lant�s�n� ve DbContext'i servislere ekle
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
