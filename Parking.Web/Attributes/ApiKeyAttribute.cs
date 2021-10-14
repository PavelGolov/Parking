using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Parking.Core.Managers;
using Parking.SharedKernel.Models;
using System;
using System.Threading.Tasks;

namespace Parking.Web.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        public string? Roles { get; set; }

        private const string ApiKeyName = "ApiKey";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey))
            {
                context.Result = GetContentResult(401, "Api Key was not provided");
                return;
            }

            var errorContent = GetContentResult(401, "Api Key is not valid");

            if (!Guid.TryParse(extractedApiKey[0], out var extractedGuid))
            {
                context.Result = errorContent;
                return;
            }

            var _userTokenManager = context.HttpContext.RequestServices.GetRequiredService<UserTokenManager>();
            var _userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
            var token = _userTokenManager.GetToken(extractedGuid);

            var isTokenInvalid = token is null || (Roles != null && !await _userManager.IsInRoleAsync(token.User, Roles));

            if (isTokenInvalid)
            {
                context.Result = errorContent;
                return;
            }

            await next();
        }

        private ContentResult GetContentResult(int? statusCode, string content)
        {
            return new ContentResult()
            {
                StatusCode = statusCode,
                Content = content
            };
        }
    }
}