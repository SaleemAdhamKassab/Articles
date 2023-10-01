using Microsoft.AspNetCore.Identity;

namespace ArticlesProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicturePath { get; set; }
        public string Bio { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twiter { get; set; }

        public List<Article> Articles { get; set; }
    }
}