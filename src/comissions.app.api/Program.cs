using System.Reflection;
using System.Security.Claims;
using comissions.app.api.Middleware;
using comissions.app.api.Middleware.Authentication;
using comissions.app.api.Services.Payment;
using comissions.app.api.Services.Storage;
using ArtPlatform.Database;
using Auth0.AspNetCore.Authentication;
using comissions.app.database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSingleton<IStorageService,ImgCdnStorageServiceProvider>();
builder.Services.AddSingleton<IPaymentService,StripePaymentServiceProvider>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Documentation",
        Version = "v1.0",
        Description = ""
    });
    options.ResolveConflictingActions(x => x.First());
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        BearerFormat = "JWT",
        Flows = new OpenApiOAuthFlows
        {
            Implicit  = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri($"{builder.Configuration.GetValue<string>("Auth0:Domain")}oauth/token"),
                AuthorizationUrl = new Uri($"{builder.Configuration.GetValue<string>("Auth0:Domain")}authorize?audience={builder.Configuration.GetValue<string>("Auth0:Audience")}"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenId" },
                    { "email", "Email" },
                    { "profile", "Profile" },
                    { "read:user",  "Read your profile information." },
                    { "write:user", "Update your profile information." },
                    { "read:billing-information", "Read your billing information." },
                    { "write:billing-information", "Update your billing information." },
                    { "read:seller-profile", "Read your seller profile information."},
                    { "write:seller-profile", "Update your seller profile information."},
                    { "write:seller-profile-request", "Accept seller profile requests."},
                    { "read:seller-profile-request", "Read seller profile requests."},
                    { "read:seller-service", "Read services on your seller profile."},
                    { "write:seller-service", "Update services on your seller profile."},
                    { "write:orders", "Create new orders and take action against existing ones."},
                    { "read:orders", "View your orders."},
                    { "read:seller-orders", "View orders on your seller profile."},
                    { "write:seller-orders", "Update orders on your seller profile."}
                }
            }
        }
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new[] { "openid", "email", "profile" }
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

});

builder.Services.AddControllers()
    .AddJsonOptions(options=>
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        );

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = $"{builder.Configuration.GetValue<string>("Auth0:Domain")}";
    options.Audience = $"{builder.Configuration.GetValue<string>("Auth0:Audience")}";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    
    options.AddPolicy("read:user", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:user", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("write:user", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:user", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:billing-information", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:billing-information", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("write:billing-information", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:billing-information", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:seller-profile", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:seller-profile", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("write:seller-profile", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:seller-profile", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:seller-profile-request", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:seller-profile-request", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("write:seller-profile-request", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:seller-profile-request", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:seller-service", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:seller-service", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("write:seller-service", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:seller-service", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("write:orders", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:orders", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("read:orders", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:orders", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    
    options.AddPolicy("read:seller-orders", policy => policy.Requirements.Add(new 
        HasScopeRequirement("read:seller-orders", builder.Configuration.GetValue<string>("Auth0:Domain"))));
    options.AddPolicy("write:seller-orders", policy => policy.Requirements.Add(new 
        HasScopeRequirement("write:seller-orders", builder.Configuration.GetValue<string>("Auth0:Domain"))));
});

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(settings =>
{
    if (app.Environment.IsDevelopment())
    {
        settings.OAuthClientId(builder.Configuration.GetValue<string>("Auth0:ClientId"));
        settings.OAuthClientSecret(builder.Configuration.GetValue<string>("Auth0:ClientSecret"));
        settings.OAuthUsePkce();
    }
});
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html"); // replace 'yourf
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<UserMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
