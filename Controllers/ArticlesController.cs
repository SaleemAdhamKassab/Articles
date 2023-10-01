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
    public class ArticlesController : Controller
    {
        private readonly IArticlesRepo _articlesRepo;
        private readonly ICategoriesRepo _categoriesRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;



        public ArticlesController(IArticlesRepo articlesRepo,
                                  ICategoriesRepo categoriesRepo,
                                  UserManager<ApplicationUser> userManager,
                                  Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _articlesRepo = articlesRepo;
            _categoriesRepo = categoriesRepo;
            _userManager = userManager;
            _environment = environment;
        }


        //Get
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return View(await _articlesRepo.Get());
            else if (await _userManager.IsInRoleAsync(user, "User"))
                return View(await _articlesRepo.Get(user.Id));
            else
                return View();
        }


        //Details
        //public async Task<IActionResult> Details(int id)
        //{
        //    ArticleDetailesViewModel model = await _articlesRepo.Details(id);

        //    if (model == null)
        //        return NotFound();

        //    return View(model);
        //}



        //Search
        public async Task<IActionResult> Search(string title)
        {
            if (!String.IsNullOrEmpty(title))
            {
                List<ArticleDetailesViewModel> articles = await _articlesRepo.Search(title);

                if (articles.Count > 0)
                    return View(nameof(Index), articles);
            }

            return RedirectToAction(nameof(Index));
        }


        //Create
        public async Task<IActionResult> Create()
        {
            AddArticleViewModel model = new()
            {
                Categories = await _categoriesRepo.Get()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddArticleViewModel model)
        {

            if (!ModelState.IsValid)
                return BadRequest("Invalid Model");

            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (loggedUser == null)
                return BadRequest("Signin first to add Article!");

            if (!UploadImage.AllowedExtenstions.Contains(Path.GetExtension(model.Image.FileName).ToLower()))
                return BadRequest("Only .png, jpeg and .jpg images are allowed!");

            if (model.Image.Length > UploadImage.MaxAllowedImageSize)
                return BadRequest("Max allowed size for poster is 1MB!");


            try
            {
                var imagePath = Path.Combine(_environment.ContentRootPath, UploadImage.ArticlesImagesFolderLocation, model.Image.FileName);

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    await model.Image.CopyToAsync(fileStream);

                Article article = new()
                {
                    Title = model.Title,
                    Content = model.Content,
                    ImagePath = imagePath,
                    CategoryId = model.CategoryId,
                    ApplicationUserId = loggedUser.Id
                };

                await _articlesRepo.Create(article);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return View(model);
            }
        }


        //Edit
        public async Task<IActionResult> Edit(int id)
        {
            ArticleDetailesViewModel article = await _articlesRepo.Details(id);
            EditArticleViewModel model = new()
            {
                Id = article.Id,
                Title = article.Title,
                Category = article.Category,
                Content = article.Content
            };

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditArticleViewModel model)
        {
            ApplicationUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (loggedUser == null)
                return BadRequest($"Signin first to Edit the Article {model.Title}!");

            Article article = await _articlesRepo.Get(id);

            if (article == null)
                return BadRequest($"The Article ({model.Title}) doesn't exists!");

            try
            {
                article.Id = model.Id;
                article.Title = model.Title;
                article.Content = model.Content;

                _articlesRepo.Edit(article);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            ArticleDetailesViewModel model = await _articlesRepo.Details(id);

            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ArticleDetailesViewModel model)
        {
            Article article = await _articlesRepo.Get(model.Id);

            if (article == null)
                return NotFound();

            _articlesRepo.Delete(article);
            
            {
                if (System.IO.File.Exists(article.ImagePath))
                    System.IO.File.Delete(article.ImagePath);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}