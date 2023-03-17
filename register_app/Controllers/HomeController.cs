using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using register_app.Models;
using register_app.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace register_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IFormService FormService { get; }
        public HomeController(IFormService formService, ILogger<HomeController> logger)
        {
            FormService = formService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            try
            {
                ViewBag.Forms = await FormService.GetAllFormsAsync(HttpContext.User);
                return View();
            }
            catch (ArgumentNullException ex)
            {
                var properties = new AuthenticationProperties { RedirectUri = "/" };
                await HttpContext.ChallengeAsync(GoogleOpenIdConnectDefaults.AuthenticationScheme, properties);
                var authResult = await HttpContext.AuthenticateAsync(GoogleOpenIdConnectDefaults.AuthenticationScheme);
                if (authResult.Succeeded)
                {
                    var newRefreshToken = authResult.Properties.GetTokenValue("refresh_token");
                    await FormService.SetRefreshTokenAsync(HttpContext.User, newRefreshToken);
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
