using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniProject.Library.Controllers
{
    public class ViewUsersController : Controller
    {
        // GET: ViewUsers
        public ActionResult View()
        {
            var db = new BookEntities5();
            user user = new user();
            var list = db.users.ToList();
            return View(list);
        }

        public ActionResult Details(int id)
        {
            var db = new BookEntities5();
            List<Borrow> list = db.Borrows.Where(item => item.user_id == id).OrderByDescending(item => item.issue_date).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult NotReturned(int id)
        {
            var db = new BookEntities5();
            List<Borrow> list = db.Borrows.Where(item => item.user_id == id && item.returned == "N ").ToList();
            return View(list);
        }
    }
}