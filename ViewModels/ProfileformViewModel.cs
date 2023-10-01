using System.ComponentModel.DataAnnotations;

namespace ArticlesProject.ViewModels
{
    public class ProfileformViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Bio { get; set; }

        [Required]
        public string Facebook { get; set; }

        [Required]
        public string Instagram { get; set; }

        [Required]
        public string Twiter { get; set; }

    }
}