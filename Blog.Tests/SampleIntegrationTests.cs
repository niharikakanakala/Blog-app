using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blog.WebAPI.Data;
using Blog.WebAPI.Services;
using Blog.WebAPI.IServices;
using Blog.WebAPI.Models;
using Blog.WebAPI;
using Xunit;

namespace SampleIntegrationTests.Tests
{
    public class BlogIntegrationTests
    {
        private TestServer _server;
        private HttpClient _client;

        public BlogIntegrationTests()
        {
            SetUpClient();
        }

        private void SetUpClient()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    string connectionString = "Server='localhost';Database='BlogDatabase';User='sa';Password='Test-hackerearth';";
                    services.AddDbContext<BlogContext>(options => options.UseSqlServer(connectionString));
                    services.AddTransient<IBlogService, BlogService>();
                    services.AddControllers();
                });

            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }

        private void ClearTasksTable()
        {
            using (var scope = _server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
                dbContext.Articles.RemoveRange(dbContext.Articles);
                dbContext.SaveChanges();
            }
        }

   private void SeedTestData()
        {
            
            using (var scope = _server.Host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
               
                dbContext.Database.EnsureCreated();
                if (!dbContext.Articles.Any())
                {
                    dbContext.Articles.AddRange(new List<Article>
                    {
                        new Article { Title = "Blog 1", Content = "This is good", Author = "person1", PublicationDate= DateTime.Now, Tags = "string1" },
                        new Article { Title = "Blog 2", Content = "This is awesome", Author = "person2", PublicationDate= DateTime.Now, Tags = "string2" }
                    });
                    dbContext.SaveChanges();
                }
            }
        }
    
        [Fact]
        public async Task TestAddBlog()
        {
            var newArticle = new Article
            {
                Title = "Blog 1", 
                Content = "This is good", 
                Author = "person1",  
                Tags = "string1"
            };

            var jsonString = JsonConvert.SerializeObject(newArticle);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/blog", httpContent);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
       
        [Fact]
        public async Task TestYourIntegrationScenario()
        {
            // Add your integration test scenario here
            // For example:
            var response = await _client.GetAsync("/api/blog");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var tasks = JsonConvert.DeserializeObject<List<Article>>(content);
            tasks.Should().NotBeNull();
        }
    }
}
