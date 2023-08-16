using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blog.WebAPI.Data;
using Blog.WebAPI.IServices;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.Services
{
    public class BlogService : IBlogService
    {
        private readonly BlogContext _blogContext;

        public BlogService(BlogContext blogContext)
        {
            _blogContext = blogContext;
        }

        public async Task<List<Article>> GetAllArticles()
        {
            return await _blogContext.Articles.ToListAsync();
        }
        public async Task<Article> GetArticleById(int id)
        {
            return await _blogContext.Articles.FindAsync(id);
        }

        public async Task CreateArticle(Article task)
        {
            _blogContext.Articles.Add(task);
            await _blogContext.SaveChangesAsync();
        }

        public async Task UpdateArticle(Article task)
        {
            _blogContext.Articles.Update(task);
            await _blogContext.SaveChangesAsync();
        }

        public async Task DeleteArticle(int id)
        {
            var task = await _blogContext.Articles.FindAsync(id);
            if (task != null)
            {
                _blogContext.Articles.Remove(task);
                await _blogContext.SaveChangesAsync();
            }
        }
        
        public async Task DeleteAllArticles()
        {
            var allTasks = await _blogContext.Articles.ToListAsync();
            _blogContext.Articles.RemoveRange(allTasks);
            await _blogContext.SaveChangesAsync();
        }
    }
}
