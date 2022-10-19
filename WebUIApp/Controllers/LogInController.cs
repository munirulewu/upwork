using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebUIApp.Data;
using WebUIApp.Models;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebUIApp.Controllers
{
    public class LogInController : Controller
    {


        private readonly DataContext _db;
        private IConfiguration _configaration;
        private readonly ILogger<LogInController> _logger;
        public LogInController(ILogger<LogInController> logger, DataContext db, IConfiguration configaration)
        {
            _logger = logger;
            _db = db;
            _configaration = configaration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserLogIn____(UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                var data = _db.UserLogIn.Where(m => m.username.Equals(userInfo.username) && (m.password.Equals(userInfo.password))).FirstOrDefault();
                if (data != null)
                {
                    bool isValid = (data.username == userInfo.username && data.password == userInfo.password);
                    if (isValid)
                    {
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userInfo.username) },
                            CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        HttpContext.Session.SetString("userName", userInfo.username);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["erroeMessage"] = "Invalid attempt";
                    return View(userInfo);
                }
            }
            else
            {
                TempData["erroeMessage"] = "Invalid attempt";
                return RedirectToAction("Index");
            }
            return View(userInfo);
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserLogIn(UserInfo objUser)
        {
            
            SqliteCommand oCmd = new SqliteCommand();
            DataTable table = new DataTable();
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            int iVal = 0;
            using (SqliteConnection conn = new SqliteConnection(Connection))
            {
                conn.Open();
                string ssQl = " select count(*) ttl from userLogIn where username='" +objUser.username+"' and password='"+objUser.password+"'" ;

                oCmd = conn.CreateCommand();
                oCmd.CommandText = ssQl;
                Object obj = oCmd.ExecuteScalar();
                iVal = Convert.ToInt16(obj);

                if (iVal == 1)
                {
                    var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, objUser.username) },
                           CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    HttpContext.Session.SetString(Constants.LOGIN_USER, objUser.username);
                    return RedirectToAction("Index", "Home");

                    //HttpContext.Session.SetString(Constants.LOGIN_USER,"yes");
                    //return RedirectToAction("Index","Home");
                }
                else
                {
                   TempData["errormessage"]= "Invalid User";
                    return RedirectToAction("Index");
                }

            }

        }

       
        public IActionResult Logout()
        {

           HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var storedCookies = Request.Cookies.Keys;
            foreach (var cookied in storedCookies)
            {
                Response.Cookies.Delete(cookied);
            }
            return RedirectToAction("Index","LogIn");

        }


    }
}
