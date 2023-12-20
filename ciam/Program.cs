using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

//add api doc
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "New Swagger",
        Description = "New Swagger Document",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});

// Configure session options
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1); // Set session timeout
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//config sso keycloack
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
})
.AddOpenIdConnect(options =>
{
    //login ciam
    options.Authority = "https://logindev.atrbpn.go.id/auth/realms/ciam/";
    options.ClientId = "ciam-web-client";
    options.ClientSecret = "42ec18b6-199f-4229-b9b9-014c482ec3ed";

    //login ppat
    //options.Authority = "https://logindev.atrbpn.go.id/auth/realms/ppat/";
    //options.ClientId = "ppat-application";
    //options.ClientSecret = "54674c49-ab91-4a7a-8eb2-257dad7dd6ad";

    options.ResponseType = "code";
    options.SaveTokens = true;
    options.CallbackPath = "/signin-oidc"; // Set the callback path
    options.SignedOutCallbackPath = "/signout-callback-oidc"; // Set the signout callback path
    options.Scope.Add("openid");
});

// Add MVC services
builder.Services.AddMvc();
builder.Services.AddMvc().AddRazorRuntimeCompilation();

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
app.UseSession();

// Add routes for callback handling
app.Map("/signin-oidc", signinApp =>
{
    signinApp.Run(async context =>
    {
        // Handle the callback from Keycloak after successful authentication
        await context.Response.WriteAsync("Authentication successful");
    });
});

app.Map("/signout-callback-oidc", signoutApp =>
{
    signoutApp.Run(async context =>
    {
        // Handle the callback from Keycloak after sign-out
        await context.Response.WriteAsync("Sign-out successful");
    });
});

app.MapControllers();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();

    endpoints.MapControllerRoute(
        name: "forgot",
        pattern: "forgot",
        defaults: new { controller = "Account", action = "Forgot" }
    );

    endpoints.MapControllerRoute(
        name: "register",
        pattern: "register",
        defaults: new { controller = "Account", action = "Register" }
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{p1?}/{p2?}");

    // Add route for Keycloak authentication callback
    endpoints.MapControllerRoute(
        name: "login-callback",
        pattern: "login-callback",
        defaults: new { controller = "Account", action = "LoginCallback" });
});

app.UseSwagger(options =>
{
    options.RouteTemplate = "api/doc/swagger/{documentname}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api/doc/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "api/doc";
    options.DocumentTitle = "MySwagger";
});
app.Run();