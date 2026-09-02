// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Session;
using NHA.Website.Software.Entities.Social_Entities;

namespace NHASoftware.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly ApplicationDbContext _context;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

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
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var userPostIds = await _context.Set<Post>()
                .Where(x => x.UserId == userId && x.Id.HasValue)
                .Select(x => x.Id!.Value)
                .ToListAsync();

            if (userPostIds.Count > 0)
            {
                var childPosts = await _context.Set<Post>()
                    .Where(x => x.ParentPostId.HasValue && userPostIds.Contains(x.ParentPostId.Value))
                    .ToListAsync();

                foreach (var childPost in childPosts)
                {
                    childPost.ParentPostId = null;
                }

                var postImages = await _context.Set<PostImage>()
                    .Where(x => x.PostId.HasValue && userPostIds.Contains(x.PostId.Value))
                    .ToListAsync();

                var reportedPosts = await _context.Set<ReportedPost>()
                    .Where(x => (x.PostId.HasValue && userPostIds.Contains(x.PostId.Value)) || x.UserId == userId)
                    .ToListAsync();

                var hiddenPosts = await _context.Set<HiddenPost>()
                    .Where(x => userPostIds.Contains(x.PostId))
                    .ToListAsync();

                var userPosts = await _context.Set<Post>()
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (postImages.Count > 0)
                {
                    _context.Set<PostImage>().RemoveRange(postImages);
                }

                if (reportedPosts.Count > 0)
                {
                    _context.Set<ReportedPost>().RemoveRange(reportedPosts);
                }

                if (hiddenPosts.Count > 0)
                {
                    _context.Set<HiddenPost>().RemoveRange(hiddenPosts);
                }

                _context.Set<Post>().RemoveRange(userPosts);
                await _context.SaveChangesAsync();
            }

            var sessionHistoryEvents = await _context.Set<SessionHistoryEvent>()
                .Where(x => x.userId == userId)
                .ToListAsync();

            if (sessionHistoryEvents.Count > 0)
            {
                _context.Set<SessionHistoryEvent>().RemoveRange(sessionHistoryEvents);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
