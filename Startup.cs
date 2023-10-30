// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Configuration;
using MyIgniteApi.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using MyIgniteApi.Hangfire;

namespace MyIgniteApi
{
    public class Startup
    {
        private string HangfireUser = "";
        private string HangfirePass = "";
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<UniversityService>();

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    // UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfire(config => config.UseMemoryStorage());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            // Membaca konfigurasi dari appsettings.json.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // HangfireUser = configuration["Hangfire:User"] ?? "";
            // HangfirePass = configuration["Hangfire:Pass"] ?? "";
            // app.UseHangfireDashboard("/hangfire", new DashboardOptions
            // {
            //     Authorization = new [] { new HangfireCustomBasicAuthenticationFilter(
            //         username : HangfireUser, 
            //         password : HangfirePass)}
            // });
            app.UseHangfireDashboard("/hangfire");
        }
    }
}
