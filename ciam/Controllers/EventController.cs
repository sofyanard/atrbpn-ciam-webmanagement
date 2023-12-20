using Ciam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Ciam.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("host") == null)
            {
                return Redirect("config");
            }

            ViewData["data"] = null;
            ViewData["code"] = null;
            ViewData["msg"] = "";

            var options = new RestClientOptions(HttpContext.Session.GetString("host"))
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/events");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<EventModel[]>(request);
                ViewData["data"] = response.Data;
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }
            return View();
        }
    }
}
