// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.SessionHistory;

namespace NHASoftware.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly IActiveSessionTracker _sessionTracker;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, IActiveSessionTracker activeSessionTracker)
        {
            _signInManager = signInManager;
            _logger = logger;
            _sessionTracker = activeSessionTracker;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var currentPrinciple = _signInManager.Context.User;
            var user = await _signInManager.UserManager.GetUserAsync(currentPrinciple);

            if (user != null)
            {
                await _sessionTracker.CreateLogoutEvent(user.Email!);
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");




            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
