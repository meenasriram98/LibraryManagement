using MiniProject.Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace MiniProject.Library.Controllers
{
    public class UserRegisterController : Controller
    {
        // GET: UserRegister
       
        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(Registration user)
        {
            string type = "";
            if (user.type == "faculty")
            {
                type = "F";
            }
            else if(user.type=="admin")
            {
                type = "A";
            }
            else
            {
                type = "S";
            }
            var db = new BookEntities5();
            Account acc = new Account();
            Account acc1 = new Account();
            user new_user = new user();
            acc = db.Accounts.Where(item => item.username == user.email).FirstOrDefault();
            if (acc != null)
            {
                ViewBag.error = "user with the email already exists";
            }
            else
            {
                Account new_acc = new Account();
                new_acc.type = type;
                new_acc.username = user.email;
                new_acc.password = user.password;
                new_acc.books_blocked = 0;
                new_acc.books_borrowed = 0;
                db.Accounts.Add(new_acc);
                db.SaveChanges();
                if(user.type=="student"||user.type=="faculty")
                {
                    acc1 = db.Accounts.Where(item => item.username == user.email).FirstOrDefault();
                    int id = acc1.id;
                    new_user.id = id;
                    new_user.name = user.name;
                    new_user.email = user.email;
                    new_user.phone_number = user.number;
                    new_user.department = user.department;
                    db.users.Add(new_user);
                    db.SaveChanges();
                }
                else
                {
                    acc1= db.Accounts.Where(item => item.username == user.email).FirstOrDefault();
                    int id = acc1.id;
                    Admin admin = new Admin();
                    admin.id = id;
                    admin.email = user.email;
                    admin.phone_number = user.number;
                    admin.name = user.name;
                    db.Admins.Add(admin);
                    db.SaveChanges();
                }
                
            }
            return View();
        }
    }
}