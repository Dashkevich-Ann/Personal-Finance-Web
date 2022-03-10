using DataLayer.Data;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Text.Json;

namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var dataContext = host.Services.GetService<DbContext>();
            var userRepository = host.Services.GetService<IRepository<User>>();

            var users = userRepository.Query().ToList();


            Console.WriteLine($"User Count {JsonSerializer.Serialize(users)}");
            Console.WriteLine(Environment.NewLine + Environment.NewLine);
            var newUser = new User
            {
                FirstName = "Test",
                Email = "test@test.com",
                Password = "test",
                Login = "test",
                DateOfBirth = DateTime.Now.AddYears(-20)
            };

            userRepository.Create(newUser);
            dataContext.SaveChanges();

            users = userRepository.Query().ToList();

            Console.WriteLine($"User Count {JsonSerializer.Serialize(users)}");

            Console.ReadKey();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
               .UseConsoleLifetime()
               //.ConfigureHostConfiguration(configHost =>
               //{
               //    configHost.SetBasePath(AppContext.BaseDirectory);
               //    configHost.AddEnvironmentVariables(prefix: "NETCORE_");
               //    configHost.AddCommandLine(args);
               //})
               .ConfigureAppConfiguration((hostContext, configApp) =>
               {
                   configApp.SetBasePath(AppContext.BaseDirectory);
                   configApp.AddEnvironmentVariables(prefix: "NETCORE_");
                   configApp.AddJsonFile("appsetting.json", optional: true);
                   configApp.AddCommandLine(args);
               })
               .ConfigureServices((hostContext, services) =>
               {
                   services
                       .AddDbContext<DbContext, ApplicationDbContext>(options => options
                               .UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")),
                           ServiceLifetime.Singleton
                       );
                   services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
               });

    }
}
