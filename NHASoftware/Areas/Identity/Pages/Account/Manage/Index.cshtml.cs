// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.FileExtensionValidator;

namespace NHASoftware.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IFileExtensionValidator _fileExtensionValidator;

        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            ApplicationDbContext context, IWebHostEnvironment environment, IFileExtensionValidator fileExtensionValidator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _hostEnvironment = environment;
            _fileExtensionValidator = fileExtensionValidator;
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
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Display Name")]
            public string DisplayName {get;set;}

            public IFormFile ProfilePicture { get; set;}
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var display = user.DisplayName;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                DisplayName = display,
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

        public async Task<IActionResult> OnPostAsync()
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

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            bool PictureUpdated = TryUpdateProfilePicture(user);
            bool DisplayNameUpdated = TryUpdateDisplayName(user);

            if (!PictureUpdated || !DisplayNameUpdated)
            {
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        /// <summary>
        /// Tries to Update the Users Display Name
        /// </summary>
        /// <param name="user">The User to update</param>
        /// <returns></returns>
        private bool TryUpdateDisplayName(ApplicationUser user)
        {
            if(Input.DisplayName != user.DisplayName)
            {
                var currentUser = _context.Users.Find(user.Id);
                currentUser.DisplayName = Input.DisplayName;
                var dataChanges = _context.SaveChanges();

                if(dataChanges == 0)
                {
                    StatusMessage = "Unexpected error happpened when trying to update Display Name";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to update the Users Profile Picture. 
        /// </summary>
        /// <param name="user">The User to update.</param>
        /// <returns></returns>
        private bool TryUpdateProfilePicture(ApplicationUser user)
        {
            if(Input.ProfilePicture != null)
            {
                //Getting the user & updating the profile picture photo path in user database. 
                var updatedUser = _context.Users.Find(user.Id);
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "ProfilePictures");

                //Checking for file extension validation. 
                if (_fileExtensionValidator.CheckValidImageExtensions(Input.ProfilePicture.FileName))
                {
                    //Delete old profile picture from files.
                    if (updatedUser!.ProfilePicturePath != null)
                    {
                        string oldProfilePicturePath = Path.Combine(uploadsFolder, updatedUser.ProfilePicturePath);

                        if (System.IO.File.Exists(oldProfilePicturePath))
                        {
                            System.IO.File.Delete(oldProfilePicturePath);
                        }
                        else
                        {
                            StatusMessage = "Error Unable to locate profile picture despite, Profile Picture Path being populated";
                        }
                    }

                    //Assigning unique GUID + filename to create unique name for path. 
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Input.ProfilePicture.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    //Writes the file to the path
                    Input.ProfilePicture.CopyTo(new FileStream(filePath, FileMode.Create));

                    updatedUser.ProfilePicturePath = uniqueFileName;
                    var dataChanges = _context.SaveChanges();

                    if (dataChanges == 0)
                    {
                        StatusMessage = "Error Unexpected error happened when trying to save changes to database. 0 Changes made to database!";
                        return false;
                    }

                    return true;
                }
                else
                {
                    StatusMessage = "Error File Extension did not match valid image extensions";
                    return false;
                }

            }

            return false;
        }
    }

}
