using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Authn.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AuthnDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthnDbContext") ?? throw new InvalidOperationException("Connection string 'AuthnDbContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UserService>();

// This code is so important dont forget it. It is a middleware for identify to roles
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}
)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/denied";
        options.Events = new CookieAuthenticationEvents()
        {

            OnSigningIn = async context =>
            {
                var scheme = context.Properties.Items.Where(k => k.Key == ".AuthScheme").FirstOrDefault();
                var claim = new Claim(scheme.Key,scheme.Value);
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity; // We did cast, I dont know why?
                var userService = context.HttpContext.RequestServices.GetRequiredService(typeof(UserService)) as UserService;
                var nameIdentifier = claimsIdentity?.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier)?.Value;
                if(userService != null & nameIdentifier != null)
                {
                    var appUser = userService.GetUserExternalProvider(scheme.Value, nameIdentifier);
                    if (appUser == null)
                    {
                        appUser = userService.AddNewUser(scheme.Value, claimsIdentity.Claims.ToList());
                    }
                    foreach(var r in appUser.RoleList)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, r));
                    }
                }
                claimsIdentity.AddClaim(claim);
            }
            //OnSigningIn = async context =>
            //{
            //    var principal = context.Principal;
            //    if (principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            //    {
            //        if (principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "samet")
            //        {
            //            var claimsIdentity = principal.Identity as ClaimsIdentity;
            //            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            //        }
            //    }
            //    await Task.CompletedTask;
            //}
        };
    }).AddOpenIdConnect("google", options =>

    {
        options.Authority = builder.Configuration["Authentication:Google:Authority"];
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"];
        options.SaveTokens = true; //Not neccesary everytime. Especially for microservices
      

    }).AddOpenIdConnect("okta", options =>
    {
        options.Authority = builder.Configuration["Authentication:Okta:Authority"];
        options.ClientId = builder.Configuration["Authentication:Okta:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Okta:ClientSecret"];
        options.CallbackPath = builder.Configuration["Authentication:Okta:CallbackPath"];
        options.ResponseType = builder.Configuration["Authentication:Okta:ResponseType"]; 
        options.SaveTokens = false;
        options.SignedOutCallbackPath = builder.Configuration["Authentication:Okta:SignedOutCallbackPath"];
        options.Scope.Add("openid");
        options.Scope.Add("profile");
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
