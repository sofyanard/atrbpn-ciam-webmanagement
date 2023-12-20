using Ciam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;

namespace Ciam.Controllers
{
    [Authorize]
    public class ClientController : Controller
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
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<ClientModel[]>(request);
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
            ViewData["clientId"] = Request.Form["clientId"];
            ViewData["protocol"] = Request.Form["protocol"];
            ViewData["enabled"] = Request.Form["enabled"];

            if (ViewData["clientId"] == null || ViewData["protocol"] == null || ViewData["enabled"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("add");
            }

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            request.AddJsonBody(JsonConvert.SerializeObject(
                new ClientPostModel()
                {
                    clientId = ViewData["clientId"].ToString(),
                    protocol = ViewData["protocol"].ToString(),
                    enabled = (ViewData["enabled"].ToString() == "") ? "false" : "true"
                }
            ));

            try
            {
                var response = client.Post(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil simpan data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "client";
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
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients/" + p1);
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            var sessionclient = new RestClient(options);
            var sessionrequest = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients/" + p1 + "/user-sessions");
            sessionrequest.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<ClientDataModel>(request);
                ViewData["data"] = response.Data;

                var sessionresponse = sessionclient.ExecuteGet<UserSessionClientModel[]>(sessionrequest);
                ViewData["UserSession"] = sessionresponse.Data;

                ViewData["id"] = p1;
                ViewData["clientId"] = response.Data.clientId;
                ViewData["surrogateAuthRequired"] = response.Data.surrogateAuthRequired;
                ViewData["enabled"] = response.Data.enabled;
                ViewData["alwaysDisplayInConsole"] = response.Data.alwaysDisplayInConsole;
                ViewData["clientAuthenticatorType"] = response.Data.clientAuthenticatorType;
                //string[] redirectUris = JsonConvert.DeserializeObject<string[]>(response.Data.redirectUris);
                ViewData["redirectUris"] = response.Data.redirectUris;
                ViewData["notBefore"] = response.Data.notBefore;
                ViewData["bearerOnly"] = response.Data.bearerOnly;
                ViewData["consentRequired"] = response.Data.consentRequired;
                ViewData["standardFlowEnabled"] = response.Data.standardFlowEnabled;
                ViewData["implicitFlowEnabled"] = response.Data.implicitFlowEnabled;
                ViewData["directAccessGrantsEnabled"] = response.Data.directAccessGrantsEnabled;
                ViewData["serviceAccountsEnabled"] = response.Data.serviceAccountsEnabled;
                ViewData["publicClient"] = response.Data.publicClient;
                ViewData["frontchannelLogout"] = response.Data.frontchannelLogout;
                ViewData["protocol"] = response.Data.protocol;
                ViewData["fullScopeAllowed"] = response.Data.fullScopeAllowed;
                ViewData["nodeReRegistrationTimeout"] = response.Data.nodeReRegistrationTimeout;
                //ViewData["defaultClientScopes"] = response.Data.defaultClientScopes;
                //ViewData["optionalClientScopes"] = response.Data.optionalClientScopes;
                ViewData["view"] = response.Data.access.view;
                ViewData["configure"] = response.Data.access.configure;
                ViewData["manage"] = response.Data.access.manage;

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
            ViewData["clientId"] = Request.Form["clientId"];
            ViewData["protocol"] = Request.Form["protocol"];
            ViewData["surrogateAuthRequired"] = (Request.Form["surrogateAuthRequired"].ToString() == "") ? "false" : Request.Form["surrogateAuthRequired"].ToString();
            ViewData["enabled"] = (Request.Form["enabled"].ToString() == "") ? "false" : Request.Form["enabled"].ToString();
            ViewData["alwaysDisplayInConsole"] = (Request.Form["alwaysDisplayInConsole"].ToString() == "") ? "false" : Request.Form["alwaysDisplayInConsole"].ToString();
            ViewData["clientAuthenticatorType"] = Request.Form["clientAuthenticatorType"];
            ViewData["redirectUris"] = Request.Form["redirectUris[]"];
            ViewData["notBefore"] = Request.Form["notBefore"];
            ViewData["bearerOnly"] = (Request.Form["bearerOnly"].ToString() == "") ? "false" : Request.Form["bearerOnly"].ToString();
            ViewData["consentRequired"] = (Request.Form["consentRequired"].ToString() == "") ? "false" : Request.Form["consentRequired"].ToString();
            ViewData["standardFlowEnabled"] = (Request.Form["standardFlowEnabled"].ToString() == "") ? "false" : Request.Form["standardFlowEnabled"].ToString();
            ViewData["implicitFlowEnabled"] = (Request.Form["implicitFlowEnabled"].ToString() == "") ? "false" : Request.Form["implicitFlowEnabled"].ToString();
            ViewData["directAccessGrantsEnabled"] = (Request.Form["directAccessGrantsEnabled"].ToString() == "") ? "false" : Request.Form["directAccessGrantsEnabled"].ToString();
            ViewData["serviceAccountsEnabled"] = (Request.Form["serviceAccountsEnabled"].ToString() == "") ? "false" : Request.Form["serviceAccountsEnabled"].ToString();
            ViewData["publicClient"] = (Request.Form["publicClient"].ToString() == "") ? "false" : Request.Form["publicClient"].ToString();
            ViewData["frontchannelLogout"] = (Request.Form["frontchannelLogout"].ToString() == "") ? "false" : Request.Form["frontchannelLogout"].ToString();
            ViewData["protocol"] = Request.Form["protocol"];
            ViewData["fullScopeAllowed"] = (Request.Form["fullScopeAllowed"].ToString() == "") ? "false" : Request.Form["fullScopeAllowed"].ToString();
            ViewData["nodeReRegistrationTimeout"] = Request.Form["nodeReRegistrationTimeout"];
            //ViewData["defaultClientScopes"] = Request.Form["defaultClientScopes"];
            //ViewData["optionalClientScopes"] = Request.Form["optionalClientScopes"];
            ViewData["view"] = (Request.Form["view"].ToString() == "") ? "false" : Request.Form["view"].ToString();
            ViewData["configure"] = (Request.Form["configure"].ToString() == "") ? "false" : Request.Form["configure"].ToString();
            ViewData["manage"] = (Request.Form["manage"].ToString() == "") ? "false" : Request.Form["manage"].ToString();

            if (ViewData["clientId"] == null || ViewData["protocol"] == null || ViewData["enabled"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("edit");
            }

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients/" + p1)
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            request.AddJsonBody(JsonConvert.SerializeObject(
                new ClientDataModel()
                {
                    id = p1,
                    clientId = ViewData["clientId"].ToString(),
                    surrogateAuthRequired = bool.Parse(ViewData["surrogateAuthRequired"].ToString()),
                    enabled = bool.Parse(ViewData["enabled"].ToString()),
                    alwaysDisplayInConsole = bool.Parse(ViewData["alwaysDisplayInConsole"].ToString()),
                    clientAuthenticatorType = ViewData["clientAuthenticatorType"].ToString(),
                    notBefore = int.Parse(ViewData["notBefore"].ToString()),
                    bearerOnly = bool.Parse(ViewData["bearerOnly"].ToString()),
                    consentRequired = bool.Parse(ViewData["consentRequired"].ToString()),
                    standardFlowEnabled = bool.Parse(ViewData["standardFlowEnabled"].ToString()),
                    implicitFlowEnabled = bool.Parse(ViewData["implicitFlowEnabled"].ToString()),
                    directAccessGrantsEnabled = bool.Parse(ViewData["directAccessGrantsEnabled"].ToString()),
                    serviceAccountsEnabled = bool.Parse(ViewData["serviceAccountsEnabled"].ToString()),
                    publicClient = bool.Parse(ViewData["publicClient"].ToString()),
                    frontchannelLogout = bool.Parse(ViewData["frontchannelLogout"].ToString()),
                    protocol = ViewData["protocol"].ToString(),
                    redirectUris = ViewData["redirectUris"].ToString().Split(','),
                    fullScopeAllowed = bool.Parse(ViewData["fullScopeAllowed"].ToString()),
                    nodeReRegistrationTimeout = int.Parse(ViewData["nodeReRegistrationTimeout"].ToString()),
                    access = new ClientAccessModel()
                    {
                        view = bool.Parse(ViewData["enabled"].ToString()),
                        configure = bool.Parse(ViewData["configure"].ToString()),
                        manage = bool.Parse(ViewData["manage"].ToString())
                    }
                }
            ));

            try
            {
                var response = client.Put(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil update data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "client";
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

            ViewData["redirectUris"] = ViewData["redirectUris"].ToString().Split(',');
            return View("edit");
        }

        public IActionResult Del(string p1)
        {
            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients/" + p1)
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
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "client";
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
            return View("index");
        }

        //public IActionResult Modal(string p1)
        //{
        //    var options = new RestClientOptions(HttpContext.Session.GetString("host"))
        //    {
        //        ThrowOnAnyError = true,
        //        RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        //    };

        //    var client = new RestClient(options);
        //    var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/clients/" + p1 + "user-sessions?first=0&max=5");
        //    request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));


        //    var response = client.ExecuteGet<UserSessionClientModel[]>(request);
        //    ViewData["UserSession"] = response.Data;

        //    return PartialView("_Modal");
        //}
    }
}
