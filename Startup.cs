using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MyIgniteApi.Services;

namespace MyIgniteApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Contoh untuk mengatur agar serialisasi menggunakan PascalCase
                    // Tambahkan konfigurasi lainnya jika diperlukan
                });        
            services.AddSingleton<UniversityService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}