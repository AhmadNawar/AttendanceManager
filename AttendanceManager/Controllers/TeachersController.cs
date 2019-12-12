using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManager.Extensions;
using AttendanceManager.Models;
using Newtonsoft.Json.Linq;

namespace AttendanceManager.Controllers
{
    public class TeachersController : Controller
    {
        public string  BaseUrl { get; set; } = ConfigurationManager.AppSettings["ApiUrl"];
        // For rendering View
        public ActionResult SignUp()
        {
            return View();
        }

        // For Signing users up
        [HttpPost]
        public async Task<ActionResult> SignUp(TeacherSignUpVm user)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var url = BaseUrl + "users";
                    var data = new
                        {email = user.Email, password = user.Password};
                    var response = await client.PostAsync(url, data.AsJson());
                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Failed to Sign up";
                        return View();
                    }
                    var body = await response.Content.ReadAsStringAsync();
                    var token = JObject.Parse(body);
                    Response.Cookies["token"].Value = token["token"].ToString();
                    Response.Cookies["token"].Expires = DateTime.Now.AddDays(1); // add expiry time
                }
                return RedirectToAction("GetLectures", "Sessions");
            }
            ViewBag.Message = "Failed to Sign up";
            return View();
        }
        // For rendering View 
        public ActionResult Login()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"].ToString();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(TeacherLoginVm user)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var url = BaseUrl + "users/login";
                    var data = new
                        {email = user.Email, password = user.Password};
                    var response = await client.PostAsync(url, data.AsJson());
                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Message = "Failed to Log in. Please Check entered information";
                        return View();
                    }
                    var body = await response.Content.ReadAsStringAsync();
                    var token = JObject.Parse(body);
                    Response.Cookies["token"].Value = token["token"].ToString();
                    Response.Cookies["token"].Expires = DateTime.Now.AddDays(1); // add expiry time
                }
                return RedirectToAction("GetLectures", "Sessions");
            }
            ViewBag.Message = "Failed to Sign up";
            return View();
        }
        
    }
}