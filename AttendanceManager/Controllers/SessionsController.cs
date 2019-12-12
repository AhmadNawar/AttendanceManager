using AttendanceManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AttendanceManager.Extensions;
using Newtonsoft.Json.Linq;


namespace AttendanceManager.Controllers
{
    public class SessionsController : Controller
    {
        public string BaseUrl { get; set; } = ConfigurationManager.AppSettings["ApiUrl"];

        
        public async Task<ActionResult> Index(string sessionKey)
        {
            if (sessionKey == null)
            {
                ViewBag.Message = "This session either ended or doesn't exist";
                return RedirectToAction("Index", "Home");
            }

            // TODO: Check if session exist, if no return user to index

            var session = new SessionLoginVm
            {
                SessionKey = sessionKey
            };
            ViewBag.Title = "Enter Session";
            return View(session);
        }

        [HttpPost]
        public async Task<ActionResult> JoinSession(SessionLoginVm session)
        {
        
            using (var client = new HttpClient())
            {
                var url = BaseUrl + "student";
                var data = new
                    {session_key = session.SessionKey, stid = session.StudentId, name = session.StudentFirstName};
               var result = await client.PostAsync(url, data.AsJson());

               session.Works = !result.IsSuccessStatusCode ? "Failed to join lecture, try again later" : "Joined Lecture Successfully";
               
            }

            return View(session);
        }

        public ActionResult Create()
        {
            if (Request.Cookies["token"]?.Value != null) return View();
            TempData["message"] = "You need to login to be able to create a lecture";
            return RedirectToAction("Login", "Teachers");
        }

        [HttpPost]
        public async Task<ActionResult> Create(SessionCreateVm session)
        {
            if (Request.Cookies["token"]?.Value != null)
            {
                var token = Request.Cookies["token"].Value;
                using (var client = new HttpClient())
                {
                    var url = BaseUrl + "lecture";
                    var data = new
                        {title = session.Title};
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var result = await client.PostAsync(url, data.AsJson());
                    var body = await result.Content.ReadAsStringAsync();
                    var responseData = JObject.Parse(body);
                    session.SessionKey = responseData["session_key"].ToString();
                }

                return RedirectToAction("ActiveSession", "Sessions", new {sessionKey=session.SessionKey});
            }
            TempData["message"] = "You need to login to be able to create a lecture";
            return RedirectToAction("Login", "Teachers");
            
        }

        public async Task<ActionResult> ActiveSession(string sessionKey)
        {
            /*TODO: Check if user logged in: Send request to check if session exist, get the title, load view with list of student*/
            if (Request.Cookies["token"]?.Value != null)
            {
                var token = Request.Cookies["token"].Value;
                var model = new ActiveSessionVm
                {
                    SessionKey = sessionKey,
                    Students = new List<string>()
                };
            
           
                using (var client = new HttpClient())
                {
                    var url = BaseUrl + "lecture/find?session_key=" + sessionKey;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var result = await client.GetAsync(url);
                    var body = await result.Content.ReadAsStringAsync();
                    var responseData = JArray.Parse(body);
                    foreach (var student in responseData)
                    {
                        model.Students.Add(student["name"].ToString());
                    }
                }

                // TODO: MOVE THIS TO BE READ IN JAVASCRIPT
                ViewBag.token = token;
                return View(model);
            }
            TempData["message"] = "You need to login";
            return RedirectToAction("Login", "Teachers");
        }
        public ActionResult GetLectures()
        {
            // TODO: check if user logged in: Send request to get all lectures
            return View();
        }
    }
}