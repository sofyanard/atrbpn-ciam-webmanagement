using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class ConfigController : Controller
    {
        public IActionResult Index()
        {
            ViewData["host"] = HttpContext.Session.GetString("host");
            ViewData["realm"] = HttpContext.Session.GetString("realm");
            ViewData["grand_type"] = HttpContext.Session.GetString("grand_type");
            ViewData["scope"] = HttpContext.Session.GetString("scope");
            ViewData["client_id"] = HttpContext.Session.GetString("client_id");
            ViewData["client_secret"] = HttpContext.Session.GetString("client_secret");
            ViewData["code"] = null;
            ViewData["msg"] = "";

            return View();
        }

        public IActionResult Setting()
        {
            ViewData["host"] = Request.Form["host"];
            ViewData["realm"] = Request.Form["realm"];
            ViewData["grand_type"] = Request.Form["grand_type"];
            ViewData["scope"] = Request.Form["scope"];
            ViewData["client_id"] = Request.Form["client_id"];
            ViewData["client_secret"] = Request.Form["client_secret"];

            if (ViewData["host"] == null || ViewData["realm"] == null || ViewData["grand_type"] == null || ViewData["scope"] == null || ViewData["client_id"] == null || ViewData["client_secret"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("index");
            }

            HttpContext.Session.SetString("host", ViewData["host"].ToString());
            HttpContext.Session.SetString("realm", ViewData["realm"].ToString());
            HttpContext.Session.SetString("grand_type", ViewData["grand_type"].ToString());
            HttpContext.Session.SetString("scope", ViewData["scope"].ToString());
            HttpContext.Session.SetString("client_id", ViewData["client_id"].ToString());
            HttpContext.Session.SetString("client_secret", ViewData["client_secret"].ToString());

            var options = new RestClientOptions(ViewData["host"].ToString() + "/realms/" + ViewData["realm"].ToString() + "/protocol/openid-connect/token") {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddParameter("grant_type", ViewData["grand_type"].ToString());
            request.AddParameter("scope", ViewData["scope"].ToString());
            request.AddParameter("client_id", ViewData["client_id"].ToString());
            request.AddParameter("client_secret", ViewData["client_secret"].ToString());

            try
            {
                var response = client.ExecutePost<SSOModel>(request);
                HttpContext.Session.SetString("access_token", response.Data.access_token);
                HttpContext.Session.SetInt32("expires_in", response.Data.expires_in);
                HttpContext.Session.SetInt32("refresh_expires_in", response.Data.refresh_expires_in);
                HttpContext.Session.SetString("token_type", response.Data.token_type);
                HttpContext.Session.SetString("id_token", response.Data.id_token);
                HttpContext.Session.SetString("scope", response.Data.scope);
                HttpContext.Session.SetInt32("conn", 1);

                ViewData["code"] = true;
                ViewData["msg"] = "Berhasil Tersambung";
                ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "config";
            }
            catch(Exception e)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Error : "+e.Message;

                HttpContext.Session.SetInt32("conn", 0);
            }

            return View("index");
        }
    }
}