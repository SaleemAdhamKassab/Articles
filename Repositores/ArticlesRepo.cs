using ArticlesProject.Data;
using ArticlesProject.Interfaces;
using ArticlesProject.Models;
using ArticlesProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ArticlesProject.Repositores
{
    public class ArticlesRepo : IArticlesRepo
    {
        private readonly ApplicationDbContext _db;

        public ArticlesRepo(ApplicationDbContext applicationDbContext) => _db = applicationDbContext;

        public async Task<Article> Get(int id) => await _db.Articles.FindAsync(id);

        public async Task<List<Article>> GetUserArticles(string userId) => await _db.Articles.Where(e => e.ApplicationUserId == userId).ToListAsync();

        public async Task<ArticleDetailesViewModel> Details(int id) =>
            await _db.Articles
            .Where(e => e.Id == id)
             .Select(e => new ArticleDetailesViewModel()
             {
                 Id = e.Id,
                 Title = e.Title,
                 Category = e.Category.Name,
                 Author = e.ApplicationUser.FirstName + " " + e.ApplicationUser.LastName,
                 Image = e.ImagePath.Substring(e.ImagePath.LastIndexOf('\\') + 1),
                 Content = e.Content,
                 Date = e.AddedOn
             }).FirstAsync();

        public async Task<List<ArticleDetailesViewModel>> Search(string searchCriteria) =>
           await _db.Articles
           .Where(e => e.Title.Trim().ToLower().Contains(searchCriteria.Trim().ToLower()))
            .Select(e => new ArticleDetailesViewModel()
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.Category.Name,
                Author = e.ApplicationUser.FirstName + " " + e.ApplicationUser.LastName,
                Image = e.ImagePath.Substring(e.ImagePath.LastIndexOf('\\') + 1),
                Date = e.AddedOn
            })
           .OrderByDescending(e => e.Date).ToListAsync();

        public async Task<List<ArticleDetailesViewModel>> Get() =>
            await _db.Articles
            .Select(e => new ArticleDetailesViewModel()
            {
                Id = e.Id,
                Title = e.Title,
                Category = e.Category.Name,
                Author = e.ApplicationUser.FirstName + " " + e.ApplicationUser.LastName,
                Image = e.ImagePath.Substring(e.ImagePath.LastIndexOf('\\') + 1),
                Date = e.AddedOn
            })
            .OrderByDescending(e => e.Date).ToListAsync();

        public async Task<List<ArticleDetailesViewModel>> Get(string userId) =>
         await _db.Articles
            .Where(e => e.ApplicationUserId == userId)
         .Select(e => new ArticleDetailesViewModel()
         {
             Id = e.Id,
             Title = e.Title,
             Category = e.Category.Name,
             Author = e.ApplicationUser.FirstName + " " + e.ApplicationUser.LastName,
             Image = e.ImagePath.Substring(e.ImagePath.LastIndexOf('\\') + 1),
             Date = e.AddedOn
         })
         .OrderByDescending(e => e.Date).ToListAsync();

        public async Task<List<ArticleDetailesViewModel>> Details() =>
            await _db.Articles
             .Select(e => new ArticleDetailesViewModel()
             {
                 Id = e.Id,
                 Title = e.Title,
                 Category = e.Category.Name,
                 Author = e.ApplicationUser.FirstName + " " + e.ApplicationUser.LastName,
                 Image = e.ImagePath.Substring(e.ImagePath.LastIndexOf('\\') + 1),
                 Content = e.Content,
                 Date = e.AddedOn
             })
            .OrderByDescending(e => e.Date)
            .ToListAsync();

        public async Task<Article> Create(Article article)
        {
            await _db.AddAsync(article);
            _db.SaveChanges();

            return article;
        }

        public void Edit(Article article)
        {
            _db.Update(article);
            _db.SaveChanges();
        }

        public void Delete(Article article)
        {
            _db.Remove(article);
            _db.SaveChanges();
        }
    }
}