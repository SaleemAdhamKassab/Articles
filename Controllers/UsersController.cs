﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ArticlesProject.Models;
using ArticlesProject.ViewModels;
using ArticlesProject.Interfaces;
using ArticlesProject.Helper;
using System.Net.Mail;

namespace ArticlesProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IArticlesRepo _articleRepo;
        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IArticlesRepo articlesRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _articleRepo = articlesRepo;
        }

        public async Task<IActionResult> Index()
        {
            List<UserViewModel> users = await _userManager.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Bio = user.Bio,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Twiter = user.Twiter,
                Roles = _userManager.GetRolesAsync(user).Result
            })
            .OrderBy(e => e.FirstName).ToListAsync();

            return View(users);
        }


        public async Task<IActionResult> Add()
        {
            var userRoles = await _roleManager.Roles.Select(e => new RoleViewModel { Name = e.Name }).ToListAsync();

            AddUserViewModel viewModel = new()
            {
                Roles = userRoles
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.Roles.Any(e => e.IsSelected))
            {
                ModelState.AddModelError("Roles", "Please Select at least One Role!");
                return View(model);
            }

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email Already Exists!");
                return View(model);
            }

            if (await _userManager.FindByNameAsync(new MailAddress(model.Email).User) != null)
            {
                ModelState.AddModelError("Username", "username Already Exists!");
                return View(model);
            }

            var user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = new MailAddress(model.Email).User,
                Email = model.Email,
                Bio = model.Bio,
                Facebook = model.Facebook,
                Twiter = model.Twiter,
                Instagram = model.Instagram,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                }
            }

            await _userManager.AddToRolesAsync(user, model.Roles.Where(e => e.IsSelected).Select(e => e.Name));

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            ProfileformViewModel viewModel = new()
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Bio = user.Bio,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Twiter = user.Twiter
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileformViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id.ToString() != model.Id)
            {
                ModelState.AddModelError("Email", "This Email is Already Assigned To Another User!");
                return View(model);
            }

            var userWithSameUserName = await _userManager.FindByNameAsync(new MailAddress(model.Email).User);
            if (userWithSameUserName != null && userWithSameUserName.Id.ToString() != model.Id)
            {
                ModelState.AddModelError("Username", "This Username is Already Assigned To Another User!");
                return View(model);
            }


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = new MailAddress(model.Email).User;
            user.Email = model.Email;
            user.Bio = model.Bio;
            user.Facebook = model.Facebook;
            user.Instagram = model.Instagram;
            user.Twiter = model.Twiter;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();

            List<RoleViewModel> roleViewModels = roles.Select(e => new RoleViewModel
            {
                Name = e.Name,
                IsSelected = _userManager.IsInRoleAsync(user, e.Name).Result
            }).ToList();

            UserRolesViewModel userRolesViewModel = new()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roleViewModels
            };

            return View(userRolesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.Name);

                if (!userRoles.Any(r => r == role.Name) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.Name);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            ProfileformViewModel viewModel = new()
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Bio = user.Bio,
                Facebook = user.Facebook,
                Instagram = user.Instagram,
                Twiter = user.Twiter
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProfileformViewModel model)
        {
            await _userManager.DeleteAsync(await _userManager.FindByIdAsync(model.Id));

            return RedirectToAction(nameof(Index));
        }
    }
}