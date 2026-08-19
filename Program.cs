using RENT_MVC_PROJECT.Repository;
using RENT_MVC_PROJECT.DTO;

var builder = WebApplication.CreateBuilder(args);


//var builder = WebApplication.CreateBuilder(args);
var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Home/LoginPage";   // Redirect if not logged in
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout
        options.SlidingExpiration = true; // Extend session if active
    });



// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddSession();
builder.Services.AddTransient<DecryptionRepo>();
builder.Services.AddTransient<GetDataRepo>();
builder.Services.AddTransient<Util>();
builder.Services.AddTransient<PostDataRepo>();
builder.Services.AddTransient<empDto>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseSession();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
    name: "default",
pattern: "{controller=Home}/{action=LoginPage}/{id?}");

app.Run();
