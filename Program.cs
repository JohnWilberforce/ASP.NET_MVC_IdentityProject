using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityFromScratch.Data;
using IdentityFromScratch.Areas.Identity.Data;
using IdentityFromScratch.Policy;
using IdentityFromScratch.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("IdentityFromScratchContextConnection");

builder.Services.AddDbContext<IdentityFromScratchContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<MoviesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("xxx")));

builder.Services.AddDefaultIdentity<IdentityFromScratchUser>(options => options.SignIn.RequireConfirmedAccount = true)
    //  following line needed to be added to get roles manager: order is important
    //  the AddRoles must be before the AddEntityFrameworkStores
    .AddRoles<IdentityRole>()
    // previous line was added to get rolemanager
    .AddEntityFrameworkStores<IdentityFromScratchContext>()
    ;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserManager",
        p => p.Requirements.Add(new IdentityFromScratch.Policy.SpecificUserRequirement("manager@mysite.com")));

    options.AddPolicy("RequireUserWalters",
        p => p.Requirements.Add(new IdentityFromScratch.Policy.SpecificUserRequirement("walters@rebootconsulting.com")));


    options.AddPolicy("RequireUserCarol",
        p => p.Requirements.Add(new IdentityFromScratch.Policy.SpecificUserRequirement("Carol@rebootconsulting.com")));

    options.AddPolicy("Require18OrOlder",
        p => p.Requirements.Add(new IdentityFromScratch.Policy.MinimumAgeRequirement(18)));
    options.AddPolicy("Require21OrOlder",
        p => p.Requirements.Add(new IdentityFromScratch.Policy.MinimumAgeRequirement(21)));
    options.AddPolicy("Require55OrOlder",
        p => p.Requirements.Add(new IdentityFromScratch.Policy.MinimumAgeRequirement(55)));

    options.AddPolicy("RequireFL",
       p => p.Requirements.Add(new IdentityFromScratch.Policy.SpecificStateRequirement("FL")));

    options.AddPolicy("RequireGA",
       p => p.Requirements.Add(new IdentityFromScratch.Policy.SpecificStateRequirement("GA")));
}
);
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

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
app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
