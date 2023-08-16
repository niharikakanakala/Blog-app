using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.IServices

{
    public interface IBlogService
    {
        Task<List<Article>> GetAllArticles();
        Task<Article> GetArticleById(int id);
        Task CreateArticle(Article article);
        Task UpdateArticle(Article article);
        Task DeleteArticle(int id);

        Task DeleteAllArticles();
        
    }
}
