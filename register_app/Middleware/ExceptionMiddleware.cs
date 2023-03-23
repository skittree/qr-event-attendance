using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using register_app.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace register_app.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IFormService formService)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (ArgumentNullException ex) when (ex.ParamName == "refreshToken")
            {
                await HandleExceptionAsync(context, formService);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, IFormService formService)
        {
            var properties = new AuthenticationProperties { RedirectUri = context.Request.Path };
            await context.ChallengeAsync(GoogleOpenIdConnectDefaults.AuthenticationScheme, properties);

            var authResult = await context.AuthenticateAsync(GoogleOpenIdConnectDefaults.AuthenticationScheme);

            if (authResult.Succeeded)
            {
                var newRefreshToken = authResult.Properties.GetTokenValue("refresh_token");
                await formService.SetRefreshTokenAsync(newRefreshToken);
            }
        }
    }
}
