using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.WebAPI.Data;
using Blog.WebAPI.IServices;
using Blog.WebAPI.Models;

namespace Blog.WebAPI.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Article>>> GetAllArticles()
        {
            var blogs = await _blogService.GetAllArticles();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticleById(int id)
        {
            var task = await _blogService.GetArticleById(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> AddArticle(Article article)
        {
            await _blogService.CreateArticle(article);
            return CreatedAtAction(nameof(GetArticleById), new { id = article.Id }, article);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, Article article)
        {
            if (id != article.Id)
            {
                return BadRequest();
            }

            var existingArticle = await _blogService.GetArticleById(id);
            if (existingArticle == null)
            {
                return NotFound();
            }

            existingArticle.Title = article.Title;
            existingArticle.Content = article.Content;
            existingArticle.Author = article.Author;
            existingArticle.PublicationDate = article.PublicationDate;
            existingArticle.Tags = article.Tags;


            await _blogService.UpdateArticle(existingArticle);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var task = await _blogService.GetArticleById(id);
            if (task == null)
            {
                return NotFound();
            }

            await _blogService.DeleteArticle(id);

            return NoContent();
        }
        
         [HttpDelete()]
        public async Task<IActionResult> DeleteAllArticles()
        {
            await _blogService.DeleteAllArticles();
            return NoContent();
        }
    }
}
