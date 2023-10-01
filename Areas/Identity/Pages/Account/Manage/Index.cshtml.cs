// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ArticlesProject.Models;
using ArticlesProject.Interfaces;
using ArticlesProject.ViewModels;
using Microsoft.Extensions.Hosting;
using ArticlesProject.Helper;

namespace ArticlesProject.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
             Microsoft.AspNetCore.Hosting.IHostingEnvironment environment
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            public string Bio { get; set; }
            public string Facebook { get; set; }
            public string Instagram { get; set; }
            public string Twiter { get; set; }

            [BindProperty]
            public IFormFile ProfilePicture { get; set; }

        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber,
                Bio = user.Bio,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Twiter = user.Twiter
            };

            if (user.ProfilePicturePath != null)
            {
                using var stream = System.IO.File.OpenRead(user.ProfilePicturePath);
                Input.ProfilePicture = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }


            if (Input.ProfilePicture != null)
            {
                if (!UploadImage.AllowedExtenstions.Contains(Path.GetExtension(Input.ProfilePicture.FileName).ToLower()))
                    return BadRequest("Only .png, jpeg and .jpg images are allowed!");

                if (Input.ProfilePicture.Length > UploadImage.MaxAllowedImageSize)
                    return BadRequest("Max allowed size for poster is 1MB!");


                string profillePicturePath = Path.Combine(_environment.ContentRootPath, UploadImage.UserProfilePicturesFolderLocation, Input.ProfilePicture.FileName);

                if (profillePicturePath != user.ProfilePicturePath)
                {
                    if (System.IO.File.Exists(user.ProfilePicturePath))
                        System.IO.File.Delete(user.ProfilePicturePath);
                }

                using (var fileStream = new FileStream(profillePicturePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(fileStream);
                    user.ProfilePicturePath = profillePicturePath;
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            user.Bio = Input.Bio;
            user.Facebook = Input.Facebook;
            user.Instagram = Input.Instagram;
            user.Twiter = Input.Twiter;

            if (Input.FirstName != firstName)
                user.FirstName = Input.FirstName;

            if (Input.LastName != lastName)
                user.LastName = Input.LastName;

            await _userManager.UpdateAsync(user);

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }



            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
