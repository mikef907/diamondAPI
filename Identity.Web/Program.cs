using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Common.Lib.Models.EM;
using Identity.Lib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DbContext>();
                Seed(context as IdentityContext);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Seed(IdentityContext context) {
            context.Users.Add(new User()
            {
                Email = "mikef907@gmail.com",
                Firstname = "Mike",
                Lastname = "Fullom",
                Password = "Password",
            });

            context.Users.Add(new User()
            {
                Email = "test@test.com",
                Firstname = "test",
                Lastname = "test",
                Password = "test",
            });

            context.SaveChanges();
        }
    }
}
