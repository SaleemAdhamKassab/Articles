using ArticlesProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArticlesProject.ViewModels
{
    public class AddArticleViewModel
    {
        public int CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        [BindProperty]
        public IFormFile Image { get; set; }
    }
}