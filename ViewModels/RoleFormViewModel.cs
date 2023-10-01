using System.ComponentModel.DataAnnotations;

namespace ArticlesProject.ViewModels
{
    public class RoleFormViewModel
    {
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}