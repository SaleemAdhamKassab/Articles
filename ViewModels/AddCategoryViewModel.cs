using System.ComponentModel.DataAnnotations;

namespace ArticlesProject.ViewModels
{
    public class AddCategoryViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}