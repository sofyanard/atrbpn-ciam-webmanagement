using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RestSharp;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    public class DocController : ControllerBase
    {
        [HttpPost]
        [Route("auth/admin/realms/{realms}/protocol/openid-connect/token")]
        public JsonResult Login([FromForm] string grantType, string scope, string clientID, string clientSecret)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/protocol/openid-connect/token");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", grantType);
            dict.Add("scope", scope);
            dict.Add("client_id", clientID);
            dict.Add("client_secret", clientSecret);
            request.AddBody(new FormUrlEncodedContent(dict));

            var response = client.ExecutePost(request);
            return new JsonResult(response.Content);
        }

        /*[HttpPost]
        [Route("auth/admin/realms/{realms}/protocol/openid-connect/token")]
        public JsonResult LoginPassword([FromForm] string grantType, string scope, string clientID, string clientSecret, string username, string password)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/protocol/openid-connect/token");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            var dict = new Dictionary<string, string>();
            dict.Add("grant_type", grantType);
            dict.Add("scope", scope);
            dict.Add("client_id", clientID);
            dict.Add("client_secret", clientSecret);
            dict.Add("username", username);
            dict.Add("password", password);
            request.AddBody(new FormUrlEncodedContent(dict));

            var response = client.ExecutePost(request);
            return new JsonResult(response.Content);
        }*/

        [HttpGet]
        [Route("auth/admin/realms/{realms}/users")]
        public JsonResult Users([FromQuery] string? search = null, string briefRepresentation = "true", string first = "0", string max = "20")
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/users/{id}")]
        public JsonResult UsersByID()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpPost]
        [Route("auth/admin/realms/{realms}/users")]
        public JsonResult UsersCreate([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePost(request);
            return new JsonResult(response.Content);
        }

        [HttpPut]
        [Route("auth/admin/realms/{realms}/users/{id}")]
        public JsonResult UsersByIDEdit([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePut(request);
            return new JsonResult(response.Content);
        }

        [HttpDelete]
        [Route("auth/admin/realms/{realms}/users/{id}")]
        public JsonResult UsersDelete()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.Delete(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/users/{id}/credentials")]
        public JsonResult UsersByIDCredentials()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"] + "/credentials");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpPut]
        [Route("auth/admin/realms/{realms}/users/{id}/reset-password")]
        public JsonResult UsersByIDResetPass([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"] + "/reset-password");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePut(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/roles")]
        public JsonResult Roles(string first = "0", string max = "20")
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/roles");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/roles-by-id/{id}")]
        public JsonResult RolesByID()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/roles-by-id/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpPost]
        [Route("auth/admin/realms/{realms}/roles")]
        public JsonResult RolesCreate([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/roles");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePost(request);
            return new JsonResult(response.Content);
        }

        [HttpPut]
        [Route("auth/admin/realms/{realms}/roles-by-id/{id}")]
        public JsonResult RolesByIDEdit([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/roles-by-id/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePut(request);
            return new JsonResult(response.Content);
        }

        [HttpDelete]
        [Route("auth/admin/realms/{realms}/roles-by-id/{id}")]
        public JsonResult RolesDelete()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/roles-by-id/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.Delete(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/users/{id}/role-mappings/realm/available")]
        public JsonResult UsersByIDRoles()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"] + "role-mappings/realm/available");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpPost]
        [Route("auth/admin/realms/{realms}/users/{id}/role-mappings/realm")]
        public JsonResult UsersByIDRolesCreate([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"] + "role-mappings/realm");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePost(request);
            return new JsonResult(response.Content);
        }

        [HttpDelete]
        [Route("auth/admin/realms/{realms}/users/{id}/role-mappings/realm")]
        public JsonResult UsersByIDRolesDelete()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/users/" + RouteData.Values["id"] + "role-mappings/realm");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.Delete(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/clients")]
        public JsonResult Clients([FromQuery] string? search = null, string briefRepresentation = "true", string first = "0", string max = "20")
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/clients");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpGet]
        [Route("auth/admin/realms/{realms}/clients/{id}")]
        public JsonResult ClientsByID()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/clients/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.ExecuteGet(request);
            return new JsonResult(response.Content);
        }

        [HttpPost]
        [Route("auth/admin/realms/{realms}/clients")]
        public JsonResult ClientsCreate([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/clients");
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePost(request);
            return new JsonResult(response.Content);
        }

        [HttpPut]
        [Route("auth/admin/realms/{realms}/clients/{id}")]
        public JsonResult ClientsByIDEdit([FromBody] string json)
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/clients/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);
            request.AddJsonBody(json);

            var response = client.ExecutePut(request);
            return new JsonResult(response.Content);
        }

        [HttpDelete]
        [Route("auth/admin/realms/{realms}/clients/{id}")]
        public JsonResult ClientsDelete()
        {
            var options = new RestClientOptions("https://logindev.atrbpn.go.id/auth")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + RouteData.Values["realms"] + "/clients/" + RouteData.Values["id"]);
            request.AddHeader("authorization", "Bearer " + Request.Headers.Authorization);

            var response = client.Delete(request);
            return new JsonResult(response.Content);
        }
    }
}
