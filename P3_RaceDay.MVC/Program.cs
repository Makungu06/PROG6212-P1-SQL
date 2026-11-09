
using P3_RaceDay.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Register MVC Controllers and Views
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<BlobStorageService>();
builder.Services.AddScoped<ApiService>();

// 2. Configure Session Storage
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Register HttpClient for communicating with Part 2 API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "Part 2 API BaseUrl is missing from appsettings.json.");
}

builder.Services.AddHttpClient("RaceDayApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// 4. Build Application
var app = builder.Build();

// 5. Configure Error Handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 6. Configure HTTP Pipeline
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// 7. Enable Session
app.UseSession();

// 8. Enable Authorization
app.UseAuthorization();

// 9. Configure Default MVC Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
