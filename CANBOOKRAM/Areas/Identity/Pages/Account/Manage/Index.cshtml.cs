﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CANBOOKRAM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CANBOOKRAM.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            /// 
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Display(Name = "Private Account")]
            public bool IsPrivate { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "City Name")]
            public string City { get; set; }

            [Display(Name = "Birth Date")]
            public DateTime BirthDate { get; set; }

            [Display(Name = "Profile Image")]
            public byte[] ProfilePicture { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var city = user.City;
            var birthDate = user.BirthDate;
            var profilePicture = user.ProfilePicture;
            var name = user.Name;
            var isPrivate = user.IsPrivate;


            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                City = city,
                BirthDate = birthDate,
                ProfilePicture = profilePicture,
                Name = name,
                IsPrivate = isPrivate
            };

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

        public InputModel GetInput()
        {
            return Input;
        }

        public async Task<IActionResult> OnPostAsync(InputModel ınput)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var city = user.City;
            var birthDate = user.BirthDate;
            var profilePicture = user.ProfilePicture;
            var name = user.Name;
            var isPrivate = user.IsPrivate;

            if (Input.Name != name)
            {
                user.Name = Input.Name;
                await _userManager.UpdateAsync(user);
            }

            if (Input.IsPrivate != isPrivate)
            {
                user.IsPrivate = Input.IsPrivate;
                await _userManager.UpdateAsync(user);
            }

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }


            if (Input.City != city)
            {
                user.City = Input.City;
                await _userManager.UpdateAsync(user);
            }


            if (Input.BirthDate != birthDate)
            {
                user.BirthDate = Input.BirthDate;
                await _userManager.UpdateAsync(user);
            }

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
