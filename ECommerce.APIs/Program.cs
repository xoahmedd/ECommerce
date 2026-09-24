
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Text;
using ECommerce.APIs.Errors;
using ECommerce.APIs.Helpers;
using ECommerce.APIs.Middlewares;
using ECommerce.Core.Entities.Identity;
using ECommerce.Core.Reposiroties_Contracts;
using ECommerce.Core.Services_Contracts;
using ECommerce.Repository;
using ECommerce.Repository._Identity;
using ECommerce.Repository.Basket_Repository;
using ECommerce.Repository.Data;
using ECommerce.Repository.Generic_Repository;
using ECommerce.Service.AuthService;
using ECommerce.Service.OrderService;

namespace ECommerce.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region Configure Services
            // Add services to the DI container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            // Register required web APIs services to the DI container


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            builder.Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {

            }).AddEntityFrameworkStores<ApplicationIdentityDbContext>();


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();

                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            builder.Services.AddAuthentication(/*JwtBearerDefaults.AuthenticationScheme*/ options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // this make it the default schema for all endpoints except if i made another scheme there in the endpoint
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["JWT:ValidAudience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AuthKey"] ?? string.Empty)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddScoped<IOrderService, OrderService>();

            #endregion


            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<StoreContext>();
            var _identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
            //ASK CLR FOR CREATING OBJECT FROM DbContext EXPLICITLY

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbContext.Database.MigrateAsync(); // UPDATE DATABASE
                await StoreContextSeed.SeedAsync(_dbContext);

                await _identityDbContext.Database.MigrateAsync();

                var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                await ApplicationIdentityContextSeed.SeedUsersAsync(_userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "an error has been occured while applying the migration");
            }


            #region Configure Kestrel Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();


            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}

