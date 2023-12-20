using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ciam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Ciam.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("host") == null)
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
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<RoleModel[]>(request);
                ViewData["data"] = response.Data;
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }

            return View("index");
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewData["name"] = Request.Form["name"];
            ViewData["description"] = Request.Form["description"];

            if (ViewData["name"] == null || ViewData["description"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("add");
            }

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            request.AddJsonBody(JsonConvert.SerializeObject(
                new RoleModel()
                {
                    name = ViewData["name"].ToString(),
                    description = ViewData["description"].ToString()
                }
            ));

            try
            {
                var response = client.Post(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil simpan data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "role";
                }
                else
                {
                    ViewData["code"] = false;
                    ViewData["msg"] = response.ErrorMessage;
                }
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }

            return View("add");
        }

        public IActionResult Edit(string p1)
        {
            var options = new RestClientOptions(HttpContext.Session.GetString("host"))
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles/" + p1);
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<RoleModel>(request);
                ViewData["data"] = response.Data;

                ViewData["id"] = p1;
                ViewData["name"] = response.Data.name;
                ViewData["description"] = response.Data.description;
                ViewData["composite"] = response.Data.composite;
                ViewData["clientRole"] = response.Data.clientRole;
                ViewData["containerId"] = response.Data.containerId;
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }

            return View();
        }

        public IActionResult Modify(string p1)
        {
            ViewData["name"] = Request.Form["name"];
            ViewData["description"] = Request.Form["description"];
            ViewData["composite"] = (Request.Form["composite"].ToString() == "") ? "false" : Request.Form["composite"].ToString();
            ViewData["clientRole"] = (Request.Form["clientRole"].ToString() == "") ? "false" : Request.Form["clientRole"].ToString();
            ViewData["containerId"] = Request.Form["containerId"];
            
            if (ViewData["name"] == null || ViewData["description"] == null || ViewData["containerId"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("add");
            }

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles/" + p1)
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            request.AddJsonBody(JsonConvert.SerializeObject(
                new RoleModel()
                {
                    id = p1,
                    name = ViewData["name"].ToString(),
                    description = ViewData["description"].ToString(),
                    composite = bool.Parse(ViewData["composite"].ToString()),
                    clientRole = bool.Parse(ViewData["clientRole"].ToString()),
                    containerId = ViewData["containerId"].ToString()
                }
            ));

            try
            {
                var response = client.Put(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil update data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "role";
                }
                else
                {
                    ViewData["code"] = false;
                    ViewData["msg"] = response.ErrorMessage;
                }
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }

            return View("edit");
        }

        public IActionResult Del(string p1)
        {
            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles/" + p1)
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.Delete(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil hapus data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "role";
                }
                else
                {
                    ViewData["code"] = false;
                    ViewData["msg"] = response.ErrorMessage;
                }
            }
            catch (Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : " + e.Message;
            }

            ViewData["id"] = "";
            ViewData["enabled"] = false;
            return View("edit");
        }
    }
}