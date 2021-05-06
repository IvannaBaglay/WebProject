using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using LeaderboardService.Services;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using LeaderboardService.Services.RatingService;
using LeaderboardService.Services.RatingData;

namespace LeaderboardService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Serilog.ILogger>(Serilog.Log.Logger);

            string leaderboardDb = Configuration.GetConnectionString("LeaderboardDB");
            services.AddScoped<ILeaderboardService, Leaderboard>();
            services.AddScoped<ILeaderboardData, LeaderboardData>();
            services.AddDbContext<LeaderboardContext>(options => options.UseMySql(leaderboardDb));

            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IRatingData, RatingData>();
            services.AddDbContext<RatingContext>(options => options.UseMySql(leaderboardDb));

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("BasicAuthentication", null);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
