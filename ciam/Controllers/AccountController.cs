using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Ciam.Models;
using RestSharp;

namespace Ciam.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> LoginCallback()
        {
            var authResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (authResult?.Succeeded != true)
            {
                // Handle failed authentication
                return Redirect(Environment.GetEnvironmentVariable("base_url").ToString());
            }

            // Get the access token and refresh token
            var accessToken = authResult.Properties.GetTokenValue("access_token");
            var refreshToken = authResult.Properties.GetTokenValue("refresh_token");

            // Save the tokens to the user's session or database
            HttpContext.Session.SetString("access_token", accessToken);
            HttpContext.Session.SetString("refresh_token", refreshToken);

            // Redirect the user to the desired page
            return Redirect(Environment.GetEnvironmentVariable("base_url").ToString());
        }

        public IActionResult Forgot(string type)
        {
            //if(type == "PPAT")
                return View("forgotPpat");
        }

        public IActionResult ForgotPass(string type)
        {
            string host = "https://logindev.atrbpn.go.id/auth";
            string grand_type = "client_credentials";
            string scope = "openid";

            //if(type == "PPAT")
            //{
            string realm = "ppat";
            string client_id = "ppat-application";
            string client_secret = "54674c49-ab91-4a7a-8eb2-257dad7dd6ad";
            //}

            var options = new RestClientOptions(host + "/realms/" + realm + "/protocol/openid-connect/token")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddParameter("grant_type", grand_type);
            request.AddParameter("scope", scope);
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);

            try
            {
                var response = client.ExecutePost<SSOModel>(request);
                ViewBag["msg"] = response.Data.access_token;
                ViewData["code"] = true;
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }

            return View("forgotPpat");
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
