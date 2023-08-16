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

namespace MainIntegrationTests.Tests
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
        public async Task TestCreateBlog()
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
        public async Task TestGetAllBlogs()
        {
            ClearTasksTable();
            // Add test data to the database
            var context = _server.Host.Services.GetRequiredService<BlogContext>();
            context.Articles.Add(new Article { Title = "Blog 1", Content = "This is good", Author = "person1", PublicationDate= DateTime.Now, Tags = "string1" });
            context.Articles.Add(new Article { Title = "Blog 2", Content = "This is awesome", Author = "person2", PublicationDate= DateTime.Now, Tags = "string2" });
            context.SaveChanges();

            var response = await _client.GetAsync("/api/blog");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var articles = JsonConvert.DeserializeObject<List<Article>>(content);
            articles.Should().HaveCount(2);
        }

        [Fact]
        public async Task TestUpdateBlog()
        {
            // Add test data to the database
            var context = _server.Host.Services.GetRequiredService<BlogContext>();
            var article = new Article { Title = "Blog 1", Content = "This is good", Author = "person1", PublicationDate= DateTime.Now, Tags = "string1" };
            context.Articles.Add(article);
            context.SaveChanges();

            article.Title = "Updated Blog";
            var jsonString = JsonConvert.SerializeObject(article);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/blog/{article.Id}", httpContent);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var updatedTask = await context.Articles.FindAsync(article.Id);
            updatedTask.Title.Should().Be("Updated Blog");
        }

        [Fact]
        public async Task TestDeleteTask()
        {
         try
         {
        ClearTasksTable();
        // Add test data to the database
        var context = _server.Host.Services.GetRequiredService<BlogContext>();
        var article = new Article { Title = "Blog to be deleted", Content = "This is good", Author = "person1", PublicationDate= DateTime.Now, Tags = "string1" };
        context.Articles.Add(article);
        context.SaveChanges();

        var response = await _client.DeleteAsync($"/api/blog/{article.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
       catch (Exception ex)
        {
            // Output any exceptions for debugging purposes
            Console.WriteLine($"Exception occurred during test: {ex}");
            throw; // Rethrow the exception to fail the test
        }
}

[Fact]
public async Task TestDeleteAllBlogs()
{
    // Add test data to the database
    var context = _server.Host.Services.GetRequiredService<BlogContext>();
    new Article { Title = "Blog 1", Content = "This is good", Author = "person1", PublicationDate= DateTime.Now, Tags = "string1" };
    new Article { Title = "Blog 2", Content = "This is awesome", Author = "person2", PublicationDate= DateTime.Now, Tags = "string2" };
    context.SaveChanges();

    var response = await _client.DeleteAsync("/api/blog");
    response.StatusCode.Should().Be(HttpStatusCode.NoContent);

    // Check if all tasks are deleted from the database
    var tasks = await context.Articles.ToListAsync();
    tasks.Should().BeEmpty();
}

    }
}
