using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using X_MINE.Data;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan layanan ke container DI.
builder.Services.AddControllersWithViews();

// Registrasi DbContext
builder.Services.AddDbContext<AppDBContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase")));

// Tambahkan layanan sesi
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Konfigurasi otentikasi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Login/Index"; // Ganti dengan path login Anda
		options.LogoutPath = "/Login/Logout"; // Ganti dengan path logout Anda
		options.AccessDeniedPath = "/Home/AccessDenied"; // Ganti dengan path akses ditolak Anda
	});

var app = builder.Build();

// Konfigurasi middleware HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Tambahkan middleware sesi sebelum middleware otorisasi
app.UseSession();

// Tambahkan middleware otentikasi sebelum middleware otorisasi
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
