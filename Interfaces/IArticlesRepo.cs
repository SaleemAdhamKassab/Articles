using ArticlesProject.Models;
using ArticlesProject.ViewModels;

namespace ArticlesProject.Interfaces
{
    public interface IArticlesRepo
    {
        Task<Article> Get(int id);
        Task<List<Article>> GetUserArticles(string userId);
        Task<ArticleDetailesViewModel> Details(int id);
        Task<List<ArticleDetailesViewModel>> Search(string searchCriteria);
        Task<List<ArticleDetailesViewModel>> Get();
        Task<List<ArticleDetailesViewModel>> Get(string userId);
        Task<List<ArticleDetailesViewModel>> Details();
        Task<Article> Create(Article article);
        void Edit(Article article);
        void Delete(Article article);
    }
}