using System.Globalization;
using BackstageFlow.Web.Data;
using Microsoft.EntityFrameworkCore;

CultureInfo brazilianPortuguese = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = brazilianPortuguese;
CultureInfo.DefaultThreadCurrentUICulture = brazilianPortuguese;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A conexão com o banco não foi configurada.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

await using (AsyncServiceScope scope = app.Services.CreateAsyncScope())
{
    AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DatabaseSeeder.SeedAsync(dbContext);
}

await app.RunAsync();

public partial class Program
{
}
