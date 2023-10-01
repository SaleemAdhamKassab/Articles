using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ArticlesProject.Models;
using ArticlesProject.Interfaces;
using ArticlesProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ArticlesProject.Helper;

namespace ArticlesProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticlesRepo _articlesRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ILogger<HomeController> logger, IArticlesRepo articlesRepo, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _articlesRepo = articlesRepo;
            _userManager = userManager;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task<IActionResult> Index() => View(await _articlesRepo.Details());


        //Search Article
        public async Task<IActionResult> SearchArticles(string title)
        {
            if (!String.IsNullOrEmpty(title))
            {
                List<ArticleDetailesViewModel> articles = await _articlesRepo.Search(title);

                if (articles.Count > 0)
                    return View(nameof(Index), articles);
            }

            return RedirectToAction(nameof(Index));
        }


        //Article Details
        public async Task<IActionResult> Details(int id)
        {
            ArticleDetailesViewModel model = await _articlesRepo.Details(id);

            if (model == null)
                return NotFound();

            return View(model);
        }


        //Publishers
        public async Task<IActionResult> Publishers()
        {
            List<PublisherViewModel> publishers = await _userManager.Users.Where(e => !e.FirstName.ToLower().Contains(Roles.Admin.ToLower())).Select(user => new PublisherViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Twiter = user.Twiter,
                ProfilePicture = user.ProfilePicturePath == null ? null : user.ProfilePicturePath.Substring(user.ProfilePicturePath.LastIndexOf('\\') + 1)
            })
            .OrderBy(e => e.FirstName).ToListAsync();

            return View(publishers);
        }


        public async Task<IActionResult> SearchPublishers(string publisher)
        {
            if (!String.IsNullOrEmpty(publisher))
            {
                List<PublisherViewModel> publishers =
                  await _userManager.Users
                    .Where(e => e.FirstName.Trim().ToLower().Contains(publisher.Trim().ToLower()) || e.LastName.Trim().ToLower().Contains(publisher.Trim().ToLower()))
                    .Select(user => new PublisherViewModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Bio = user.Bio,
                        Facebook = user.Facebook,
                        Instagram = user.Instagram,
                        Twiter = user.Twiter,
                        ProfilePicture = user.ProfilePicturePath == null ? null : user.ProfilePicturePath.Substring(user.ProfilePicturePath.LastIndexOf('\\') + 1)
                    })
                    .OrderBy(e => e.FirstName)
                    .ToListAsync();

                if (publishers.Count > 0)
                    return View(nameof(Publishers), publishers);
            }

            return RedirectToAction(nameof(Publishers));
        }
    }
}