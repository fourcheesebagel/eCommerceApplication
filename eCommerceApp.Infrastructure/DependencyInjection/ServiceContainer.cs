using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Authentication;
using eCommerceApp.Domain.Interfaces.Cart;
using eCommerceApp.Infrastructure.Data;
using eCommerceApp.Infrastructure.Middleware;
using eCommerceApp.Infrastructure.Repositories;
using eCommerceApp.Infrastructure.Repositories.Authentication;
using eCommerceApp.Infrastructure.Repositories.Cart;
using eCommerceApp.Infrastructure.Services;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace eCommerceApp.Infrastructure.DependencyInjection
{
    public static class ServiceContainer //Basically the Program.CS from school
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = "Default";
            services.AddDbContext<AppDbContext>(option =>
            option.UseSqlServer(config.GetConnectionString(connectionString),
            sqlOptions =>
            { //Ensure this is the correct assembly
                sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(); //Enables Automatic Retries for transient Failures
            }).UseExceptionProcessor(),
            ServiceLifetime.Scoped);

            services.AddScoped<IGeneric<Product>, GenericRepository<Product>>();
            services.AddScoped<IGeneric<Category>, GenericRepository<Category>>();
            services.AddScoped(typeof(IAppLogger<>), typeof(SerilogLoggerAdapter<>));

            //This sets up user registration and log in RULES
            services.AddDefaultIdentity<AppUser>(options => //FIRST allow normal sign up through the app itself, no social logins ie., google, microsoft, using the AppUser class which consumes microsooft identity user class
            {
                options.SignIn.RequireConfirmedEmail = true; //This will makje sure that the email of the account is confirmed before being able to log in
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider; //uses standard email system to send confirmation links
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddRoles<IdentityRole>() //Adds support for roles
                .AddEntityFrameworkStores<AppDbContext>(); //THIS SAves the user to the Database
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; //Uses JWT Token
                options.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => //BASICALLY your Digital ID Card, this will allow users to just provide this token rather than logging in everytime to access THEIR information or information that requires authentication
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true, //cheks that token is meant for MY APP
                    ValidateIssuer = true, //checks that the token was issued by MY Server
                    ValidateLifetime = true, //check if token is EXPIRED
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!))
                };
            });

            services.AddScoped<IUserManagement, UserManagement>();
            services.AddScoped<ITokenManagement, TokenManagement>();
            services.AddScoped<IRoleManagement, RoleManagement>();
            services.AddScoped<IPaymentMethod, PaymentMethodRepository>();
            services.AddScoped<IPaymentService, StripePaymentService>();

            return services;
        } 

        public static IApplicationBuilder UseInfrastructureService(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }
}
