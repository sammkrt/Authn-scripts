using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// This code is so important dont forget it. It is a middleware for identify to roles
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GoogleOpenID";
}
)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/denied";
        //options.Events = new CookieAuthenticationEvents()
        //{
        //    OnSigningIn = async context =>
        //    {
        //        var principal = context.Principal;
        //        if (principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
        //        {
        //            if (principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "samet")
        //            {
        //                var claimsIdentity = principal.Identity as ClaimsIdentity;
        //                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
        //            }
        //        }
        //        await Task.CompletedTask;
        //    }
        //};
    }).AddOpenIdConnect("GoogleOpenID", options =>

    {
        options.Authority = "https://accounts.google.com";
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/auth";
        options.SaveTokens = true; //Not neccesary everytime. Especially for microservices
        options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents()
        {
            OnTokenValidated = async context =>
            {
                if (context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "104989183017641520332") // This is my uniqe value for my email
                {
                    var claim = new Claim(ClaimTypes.Role, "Admin");
                    var claimsIdentity = context.Principal.Identity as ClaimsIdentity; // We did cast, I dont know why?
                    claimsIdentity.AddClaim(claim);
                }
                    
            }
        };

    });
    
    //.AddGoogle(options =>
    //{
    //    options.ClientId = "861039316121-e1bqv3vedb9qm790v37418sef341pbbv.apps.googleusercontent.com";
    //    options.ClientSecret = "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";
    //    options.CallbackPath = "/auth";
        
    //});

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

app.Run();
