using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Application.Services.Concrete;
using Armut.Messaging.Infrastructure.Hubs;
using Armut.Messaging.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;

namespace Armut.Messaging.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.Configure<RouteOptions>(x => x.LowercaseUrls = true);
        
        //Dependencies
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IChatService, ChatService>();
        builder.Services.AddScoped<MessageHistoryService, MessageHistoryService>();

        // MongoDb Connection
        builder.Services.AddScoped(sp =>
        {
            var connectionString = builder.Configuration.GetConnectionString("MongoDB");
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase("armut");
        });

        // JWT
        var securityKey = builder.Configuration.GetSection("Identity:TokenSecurityKey").Value;
        var keyBytes = Encoding.ASCII.GetBytes(securityKey);

        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSignalR(options => options.EnableDetailedErrors = true);

        // Versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            //options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.ExampleFilters();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                }
            });

            options.AddSignalRSwaggerGen();
        });

        builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        // SeriLog
        builder.Host.UseSerilog((context, config) =>
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            config.ReadFrom.Configuration(context.Configuration)
                .WriteTo.Elasticsearch(ConfigureElasticSink(builder.Configuration, environment))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext();
        });

        var app = builder.Build();

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

        // Middleware
        app.UseMiddleware<ApiResponseMiddleware>();
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        // Configure the HTTP request pipeline.

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<ChatHub>("/chat");
        });

        app.Run();

        // ElasticSearch Options
        ElasticsearchSinkOptions ConfigureElasticSink(ConfigurationManager configuration, string environment)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration.GetValue<string>("ElasticConfiguration:Uri")))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}