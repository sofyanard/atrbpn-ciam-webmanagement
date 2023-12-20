using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using RestSharp.Authenticators.OAuth2;

namespace Ciam.Controllers
{
    [Authorize]
    public class UserController : Controller
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
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/users");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<UserDataModel[]>(request);
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
            ViewData["firstName"] = Request.Form["firstName"];
            ViewData["lastName"] = Request.Form["lastName"];
            ViewData["username"] = Request.Form["username"];
            ViewData["email"] = Request.Form["email"];
            ViewData["password"] = Request.Form["password"];
            ViewData["enabled"] = Request.Form["enabled"];

            if (ViewData["firstName"] == null || ViewData["lastName"] == null || ViewData["username"] == null || ViewData["email"] == null || ViewData["password"] == null || ViewData["enabled"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("add");
            }

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/users")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            CredentialsModel[] cred = {
                new CredentialsModel()
                {
                    temporary = false,
                    type = "password",
                    value = ViewData["password"].ToString()
                }
            };

            request.AddJsonBody(JsonConvert.SerializeObject(
                new UserModel()
                {
                    firstName = ViewData["firstName"].ToString(),
                    lastName = ViewData["lastName"].ToString(),
                    email = ViewData["email"].ToString(),
                    enabled = (ViewData["enabled"].ToString() == "true")?"true":"false",
                    username = ViewData["userName"].ToString(),
                    credentials = cred
                }
            ));

            try
            {
                var response = client.Post(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil simpan data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "user";
                }else
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
            //Commented karena (string? p1) bernilai null apabila Action ini diakses melalui Index bukan dari modal reset password
            //Try Catch untuk Return data dari Action Reset Password.
            //try
            //{
            //    p1 = TempData["id"].ToString();
            //    ViewData["msg"] = TempData["msg"];
            //    ViewData["red"] = TempData["code"];
            //    ViewData["red"] = TempData["red"];
            //}
            //catch (Exception e)
            //{

            //}


            var options = new RestClientOptions(HttpContext.Session.GetString("host"))
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/users/"+p1);
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));


            var requestUserRole = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/users/" + p1 + "/role-mappings/realm/");
            requestUserRole.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            var requestRole = new RestRequest("/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles");
            requestRole.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            try
            {
                var response = client.ExecuteGet<UserDataModel>(request);
                ViewData["data"] = response.Data;

                ViewData["id"] = p1;
                ViewData["username"] = response.Data.username;
                ViewData["enabled"] = response.Data.enabled;
                ViewData["totp"] = response.Data.totp;
                ViewData["emailVerified"] = response.Data.emailVerified;
                ViewData["firstName"] = response.Data.firstName;
                ViewData["lastName"] = response.Data.lastName;
                ViewData["email"] = response.Data.email;
                ViewData["notBefore"] = response.Data.notBefore;
                ViewData["manageGroupMembership"] = response.Data.access.manageGroupMembership;
                ViewData["view"] = response.Data.access.view;
                ViewData["mapRoles"] = response.Data.access.mapRoles;
                ViewData["impersonate"] = response.Data.access.impersonate;
                ViewData["manage"] = response.Data.access.manage;

                var responseUserRole = client.ExecuteGet<AssignRoleModel[]>(requestUserRole);
                ViewData["assignRole"] = responseUserRole.Data;

                var responseRole = client.ExecuteGet<RoleModel[]>(requestRole);
                ViewData["listRole"] = responseRole.Data;
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
            ViewData["firstName"] = Request.Form["firstName"];
            ViewData["lastName"] = Request.Form["lastName"];
            ViewData["username"] = Request.Form["username"];
            ViewData["email"] = Request.Form["email"];
            ViewData["notBefore"] = Request.Form["notBefore"];
            ViewData["assignedRole"] = Request.Form["assignedRole[]"];
            ViewData["enabled"] = (Request.Form["enabled"].ToString() == "")?"false": Request.Form["enabled"].ToString();
            ViewData["totp"] = (Request.Form["totp"].ToString() == "") ? "false" : Request.Form["totp"].ToString();
            ViewData["emailVerified"] = (Request.Form["emailVerified"].ToString() == "") ? "false" : Request.Form["emailVerified"].ToString();
            ViewData["manageGroupMembership"] = (Request.Form["manageGroupMembership"].ToString() == "") ? "false" : Request.Form["manageGroupMembership"].ToString();
            ViewData["view"] = (Request.Form["view"].ToString() == "") ? "false" : Request.Form["view"].ToString();
            ViewData["mapRoles"] = (Request.Form["mapRoles"].ToString() == "") ? "false" : Request.Form["mapRoles"].ToString();
            ViewData["impersonate"] = (Request.Form["impersonate"].ToString() == "") ? "false" : Request.Form["impersonate"].ToString();
            ViewData["manage"] = (Request.Form["manage"].ToString() == "") ? "false" : Request.Form["manage"].ToString();

            if (ViewData["firstName"] == null || ViewData["lastName"] == null || ViewData["username"] == null || ViewData["email"] == null || ViewData["enabled"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return View("edit");
            }

            string[] roles = ViewData["assignedRole"].ToString().Split(',');
            AssignRoleModel[] assignRoleModels = new AssignRoleModel[roles.Length];

            for (int i = 0; i < roles.Length; i++)
            {
                string[] roleObjects = roles[i].Split(";");

                AssignRoleModel roleModel = new AssignRoleModel();
                roleModel.id = roleObjects[0].ToString().Trim();
                roleModel.name = roleObjects[1].ToString().Trim();
                roleModel.description = roleObjects[2].ToString().Trim();
                roleModel.composite = bool.Parse(roleObjects[3].ToString().Trim());
                roleModel.clientRole = bool.Parse(roleObjects[4].ToString().Trim());
                roleModel.containerId = roleObjects[5].ToString().Trim();

                assignRoleModels[i] = roleModel;
            };
            ViewData["assignRole"] = assignRoleModels;

            var optionsRole = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/roles")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var clientRole = new RestClient(optionsRole);
            var requestRole = new RestRequest();
            requestRole.AddHeader("Content-type", "application/json");
            requestRole.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            var responseRole = clientRole.ExecuteGet<RoleModel[]>(requestRole);

            RoleModel[] listRoles = responseRole.Data;
            RoleModel[] newlistRoles = new RoleModel[0];
            for (int i = 0; i < listRoles.Length; i++)
            {
                bool roleFound = false;
                for (int j = 0; j < assignRoleModels.Length; j++)
                {
                    if (listRoles[i].id == assignRoleModels[j].id)
                    {
                        roleFound = true;
                        break;
                    }
                }
                if (!roleFound)
                {
                    RoleModel listModel = new RoleModel();
                    listModel.id = listRoles[i].id;
                    listModel.name = listRoles[i].name;
                    listModel.description = listRoles[i].description;
                    listModel.composite = listRoles[i].composite;
                    listModel.clientRole = listRoles[i].clientRole;
                    listModel.containerId = listRoles[i].containerId;

                    Array.Resize(ref newlistRoles, newlistRoles.Length + 1);
                    newlistRoles[newlistRoles.Length - 1] = listModel;
                }
            };
            ViewData["listRole"] = newlistRoles;

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/users/" + p1)
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            
            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            request.AddJsonBody(JsonConvert.SerializeObject(
                new UserDataModel()
                {
                    id = p1,
                    firstName = ViewData["firstName"].ToString(),
                    lastName = ViewData["lastName"].ToString(),
                    email = ViewData["email"].ToString(),
                    enabled = bool.Parse(ViewData["enabled"].ToString()),
                    username = ViewData["userName"].ToString(),
                    notBefore = int.Parse(ViewData["notBefore"].ToString()),
                    access = new AccessModel()
                    {
                        impersonate = bool.Parse(ViewData["enabled"].ToString()),
                        manage = bool.Parse(ViewData["enabled"].ToString()),
                        manageGroupMembership = bool.Parse(ViewData["enabled"].ToString()),
                        mapRoles = bool.Parse(ViewData["enabled"].ToString()),
                        view = bool.Parse(ViewData["enabled"].ToString())
                    }
                }
            ));

            var roleOptions = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/users/" + p1 + "/role-mappings/realm")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var roleClient = new RestClient(roleOptions);
            var roleRequest = new RestRequest();
            roleRequest.AddHeader("Content-type", "application/json");
            roleRequest.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));
            roleRequest.AddJsonBody(assignRoleModels);

            var roleDelete = new RestRequest();
            roleDelete.AddHeader("Content-type", "application/json");
            roleDelete.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));
            roleDelete.AddJsonBody(newlistRoles);

            try
            {
                var response = client.Put(request);
                var roleResponse = roleClient.Post(roleRequest);
                var roleDeleteResponse = roleClient.Delete(roleDelete);

                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil update Role data";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "user";
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
            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/users/" + p1)
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
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "user";
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

        public IActionResult Reset(string p1)
        {
            ViewData["id"] = p1;
            ViewData["password"] = Request.Form["password"];

            if (ViewData["password"] == null)
            {
                ViewData["code"] = false;
                ViewData["msg"] = "Mohon periksa kembali inputan anda";

                return RedirectToAction("Edit");
            }

            var options = new RestClientOptions(HttpContext.Session.GetString("host") + "/admin/realms/" + HttpContext.Session.GetString("realm") + "/users/" + p1 + "/reset-password")
            {
                ThrowOnAnyError = true,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + HttpContext.Session.GetString("access_token"));

            request.AddJsonBody(JsonConvert.SerializeObject(
                new ResetPasswordModel()
                {
                    type = "password".ToString(),
                    value = ViewData["password"].ToString(),
                    temporary = bool.Parse("false".ToString()),
                }
            ));

            try
            {
                var response = client.Put(request);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent)
                {
                    ViewData["code"] = true;
                    ViewData["msg"] = "Berhasil update password";
                    ViewData["red"] = Environment.GetEnvironmentVariable("base_url") + "user";
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

            //return View("index");

            TempData["id"] = ViewData["id"];
            TempData["msg"] = ViewData["msg"];
            TempData["code"] = ViewData["code"];
            TempData["red"] = ViewData["red"];
            TempData["id"] = ViewData["id"];

            return RedirectToAction("edit");
        }
    }
}