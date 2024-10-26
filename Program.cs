using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Middlewares;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Token",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new string[] { }
                },
            });
        });

// todo AddAuthorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyType.AdminManager.ToString(), policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && (c.Value == PolicyType.Manager.ToString() || c.Value == PolicyType.Admin.ToString()))
        ));
    
    options.AddPolicy(nameof(PolicyType.Admin), policy =>
        policy.RequireRole(PolicyType.Admin.ToString()));

    options.AddPolicy(nameof(PolicyType.Manager), policy =>
        policy.RequireRole(PolicyType.Manager.ToString()));

    options.AddPolicy(nameof(PolicyType.Staff), policy =>
        policy.RequireRole(PolicyType.Staff.ToString()));

    options.AddPolicy(nameof(PolicyType.User), policy =>
        policy.RequireRole(PolicyType.User.ToString()));


    options.AddPolicy(PolicyType.AccessToken.ToString(), policy =>
        policy.RequireRole(PolicyType.AccessToken.ToString()));
});

string AllowAll = "AllowAll";

builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAll, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

ConfigureServices(builder.Services);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseCors(AllowAll);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureServices(IServiceCollection services)
{
    services.Configure<MongoDbSettings>(
       builder.Configuration.GetSection(nameof(MongoDbSettings)));

    IServiceCollection serviceCollection = services.AddSingleton(sp =>
        sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
}