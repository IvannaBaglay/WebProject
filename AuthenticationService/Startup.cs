using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using AuthenticationService.Models;
using Microsoft.AspNetCore.Authentication;
using AuthenticationService.Services;
using System;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Services.UserService;

namespace AuthenticationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger>(Log.Logger);

            string dbConnection = Configuration.GetConnectionString("AuthenticationServiceContext");

            services.AddDbContext<UserContext>(options => options.UseMySql(dbConnection));

            services.AddScoped<IUserData, UserData>();

            Program.useUbiAuth = Environment.IsProduction();

            if (Program.useUbiAuth)
            {
                services.AddScoped<IUserService, UbiUserService>();

                services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, UbiServicesAuthHandler>("BasicAuthentication", null);
            }
            else
            {
                services.AddScoped<IUserService, CustomUserService>();

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                   {
                       options.RequireHttpsMetadata = false;

                       options.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidateIssuer = true,
                           ValidIssuer = TokenManager.TokenIssuer,

                           ValidateAudience = true,
                           ValidAudience = TokenManager.TokenAudience,

                           ValidateLifetime = true,

                           IssuerSigningKey = TokenManager.GetSymetricKey(),
                           ValidateIssuerSigningKey = true,
                       };
                   });
            }

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanChangeAllUsers", policy => policy.RequireRole("admin"));
                options.AddPolicy("CanViewAllUsers", policy => policy.RequireRole("admin", "dev"));
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
