using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniProject.Library.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username,string password)
        {
            ViewBag.Error = "Invalid Credentials";
            ViewData["Error"] = "invalid credentials";
            using (var db = new BookEntities5())
            {
                Account query = new Account();
                query = db.Accounts.Where(a => a.username == username && a.password == password).FirstOrDefault();
                if (query != null)
                {
                    Session["user"] = query;
                    Session["id"] = username;
                    
                    if(query.type=="A    ")
                    {
                        Session["role"] = "admin";
                        return RedirectToAction("NewlyAddedBooks", "Admin");
                    }
                    else
                    {
                        Session["role"] = "student";
                        Session["id"] = username;
      
                        return RedirectToAction("UserHome", "User");
                    }
                    
                }
                else
                {
                    
                    return View();
                }
            }
        }
    }
}