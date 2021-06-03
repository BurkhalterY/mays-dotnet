using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Epsic.Info3e.Mays.DbContext;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using Epsic.Info3e.Mays.Seeders;
using Epsic.Info3e.Mays.Models;

namespace Epsic.Info3e.Mays.Tests.Controllers
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MaysDbContext>));
                if (descriptor == null) return;
                services.Remove(descriptor);

                services.AddDbContext<MaysDbContext>(options =>
                {
                    options.UseInMemoryDatabase("Mays.db");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MaysDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();
                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();
                    var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        ResetInMemoryDatabase(db, userManager, roleManager);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                    }
                }
            });
        }

        private static async void ResetInMemoryDatabase(MaysDbContext db,
                                                        UserManager<User> userManager,
                                                        RoleManager<IdentityRole> roleManager)
        {
            // Clear db
            db.RemoveRange(db.Posts);
            db.RemoveRange(db.Users);
            db.RemoveRange(db.Roles);
            db.RemoveRange(db.UserRoles);
            db.SaveChanges();

            var users = new List<User>
            {
                new User {
                    UserName = "pierre",
                    Email = "pierre@hotmail.com",
                },
                new User {
                    UserName = "paul",
                    Email = "paul@gmail.com",
                },
                new User {
                    UserName = "jacques",
                    Email = "jacques@bluwin.com",
                },
            };

            // Add users
            await userManager.CreateAsync(users[0], "P1erre++");
            await userManager.CreateAsync(users[1], "P4ul+-+-");
            await userManager.CreateAsync(users[2], "Jacqu3s-");

            // Add roles
            SeedDataApplicationRoles.SeedRoles(roleManager);

            // Link users and roles
            foreach (var user in users)
            {
                await userManager.AddToRoleAsync(user, "user");
            }
        }
    }

    public class ApiControllerTestBase : CustomWebApplicationFactory<Startup>
    {
        private CustomWebApplicationFactory<Startup> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetupTest()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            var response = await _client.GetAsync(url);

            var body = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(body);
        }

        protected async Task<HttpResponseMessage> PostBasicAsync<T>(string url, T body)
        {
            return await _client.PostAsJsonAsync(url, body);
        }

        protected async Task<HttpResponseMessage> PostFileAsync(string url, byte[] file)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(file), "file", "filename");
            return await _client.PostAsync(url, content);
        }

        protected async Task<T> PostAsync<T>(string url, T body)
        {
            return await PostAsync<T, T>(url, body);
        }

        protected async Task<U> PostAsync<T, U>(string url, T body)
        {
            var response = await _client.PostAsJsonAsync(url, body);

            Console.WriteLine(response);

            return await response.Content.ReadFromJsonAsync<U>();
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var response = await _client.DeleteAsync(url);

            return response.EnsureSuccessStatusCode();
        }
    }
}
