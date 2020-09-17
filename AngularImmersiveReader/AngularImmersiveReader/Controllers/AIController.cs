using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AngularImmersiveReader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularImmersiveReader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AIController : ControllerBase
    {
        private readonly AppSettingsDTO _appSettings = new AppSettingsDTO();
        public AIController(IOptions<AppSettingsDTO> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        /// <summary>
        /// Get an Azure AD authentication token
        /// </summary>
        private async Task<string> GetTokenAsync()
        {
            string authority = $"https://login.windows.net//{_appSettings.TenantId}";
            const string resource = "https://cognitiveservices.azure.com/";

            AuthenticationContext authContext = new AuthenticationContext(authority);
            ClientCredential clientCredential = new ClientCredential(_appSettings.ClientId, _appSettings.ClientSecret);

            AuthenticationResult authResult = await authContext.AcquireTokenAsync(resource, clientCredential);

            return authResult.AccessToken;
        }

        [HttpGet("get-token-and-subdomain")]
        public async Task<JsonResult> GetTokenAndSubdomain()
        {
            try
            {
                string tokenResult = await GetTokenAsync();

                return new JsonResult(new { token = tokenResult, subdomain = _appSettings.Subdomain });
            }
            catch (Exception e)
            {
                string message = "Unable to acquire Azure AD token. Check the debugger for more information.";
                Debug.WriteLine(message, e);
                return new JsonResult(new { error = message });
            }
        }
    }
}
