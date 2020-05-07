using Common.Lib;
using Common.Lib.ServiceAgent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using STS.Lib;
using ElmahCore;
using ElmahCore.Mvc;

namespace STS.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration) => Configuration = configuration;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            // Setup typed access to app settings
            var appSettingsSection = Configuration.GetSection("appSettings");
            services.Configure<AppSettings>(appSettingsSection);
            services.AddElmah();
            services.AddControllers();
            services.AddScoped<IIdentityAgent, IdentityAgent>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseElmah();
            app.UseRouting();

            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World from STS!");
                });
            });
        }
    }
}
