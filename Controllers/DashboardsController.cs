using ArticlesProject.Helper;
using ArticlesProject.Interfaces;
using ArticlesProject.Models;
using ArticlesProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArticlesProject.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class DashboardsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IArticlesRepo _articleRepo;

        public DashboardsController(UserManager<ApplicationUser> userManager, IArticlesRepo articlesRepo)
        {
            _userManager = userManager;
            _articleRepo = articlesRepo;
        }

        public async Task<IActionResult> Dashboard()
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            List<Article> articles = await _articleRepo.GetUserArticles(user.Id);

            DashboardViewModel model = new()
            {
                UserName = user.FirstName,
                TotalArticles = articles.Count,
                ArticlesForThisYear = articles.Where(e => e.AddedOn.Year == DateTime.Now.Year).Count(),
                ArticlesForThisMonth = articles.Where(e => e.AddedOn.Month == DateTime.Now.Month).Count()
            };

            return View(model);
        }
    }
}
