using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace MiniProject.Library.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult UserHome()
        {
            user user = new user();
            var db = new BookEntities5();
            ViewBag.user = Session["id"];
            user = db.users.Find(Session["id"]);
            ViewBag.name = user.name;
            return View();
        }

       
        [HttpGet]
        public ActionResult Block()
        {
            var db = new BookEntities5();
            List<Book> list = db.Books.ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult BlockBook(string id)
        {
            int flag = 0;
            var db = new BookEntities5();
            //var x = from item in db.Accounts select item;
            string email= Convert.ToString(Session["id"]);
            user user = new user();
            user = db.users.Where(item => item.email == email).FirstOrDefault();
            int user_id = user.id;
            Account acc = new Account();
            acc = db.Accounts.Where(item => item.username == email).FirstOrDefault();
            if (user.id == user_id)
            {
                var book = db.Books.Single(m => m.ISBNNO == id);
                if (acc.books_blocked >= 2 || book.copies < 0)
                {

                    TempData["CannotBlock"] = "Limit exceeds";
                    ViewData["Error"] = "Limit exceeds";

                    return RedirectToAction("UserHome");
                }
                else
                {
                    Borrow myBorrowObject = new Borrow();
                    var query = db.Copies.Where(a => a.ISBNNO == id);
                    foreach (var k in query)
                    {
                        if (k.copies_available == "Y")
                        {
                            flag = 1;
                            k.copies_available = "N";
                            myBorrowObject.copy_id = k.copy_id;
                            var query1 = db.Books.Single(a => a.ISBNNO == k.ISBNNO);
                            query1.copies = query1.copies - 1;
                            if(query1.copies<=0)
                            {
                                query1.status = "unavailable";
                            }
                            acc.books_blocked = acc.books_blocked + 1;
                            myBorrowObject.ISBNNO = id;
                            myBorrowObject.returned = "N";
                            myBorrowObject.issue_date = DateTime.Now;
                            myBorrowObject.user_id = user_id;
                            myBorrowObject.block = "Y";
                            myBorrowObject.borrow1 = "N";
                            myBorrowObject.return_date = DateTime.Now.AddHours(24);
                            db.Borrows.Add(myBorrowObject);
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        ViewBag.BlockError = "no copies available to block";
                        return RedirectToAction("UserHome");
                    }

                }

            }
            db.SaveChanges();
            TempData["BookBlocked"] = "book blocked";
            return RedirectToAction("UserHome");

        }

        [HttpGet]
        public ActionResult UserInfo()
        {
            var db = new BookEntities5();
            user user1 = new user();
            string email = Convert.ToString(Session["id"]);
            user1 = db.users.Where(item => item.email ==email).FirstOrDefault();
            //int id = user1.id;
            //user user = new user();
            //user = db.users.SingleOrDefault(item => item.id == id);
            
            return View(user1);
        }

        [HttpGet]
        public ActionResult Edit(string email)
        {
            var db = new BookEntities5();
            var std = db.users.Where(item => item.email == email).FirstOrDefault();
            return View(std);
        }

        [HttpPost]
        public ActionResult Edit(user user)
        {
            var db = new BookEntities5();
            user u1 = db.users.Find(user.email);
            u1.name = user.name;
            u1.department = user.department;
            u1.phone_number = user.phone_number;
            db.SaveChanges();
            return RedirectToAction("UserInfo");
        }

        [HttpGet]
        public ActionResult NewlyAddedBooks()
        {
            var db = new BookEntities5();
            return View(db.Books.OrderByDescending(item => item.books_date).Take(4).ToList());

        }

        //public ActionResult SearchBar(string search)
        //{
        //    BookEntities5 db = new BookEntities5();
        //    return View(db.Books.Where(x => x.name.Contains(search) || x.subject.Contains(search) || x.publisher.Contains(search) || x.author_name.Contains(search) || x.department.Contains(search) || search == null).ToList());

        //}

        [HttpGet]
        public ActionResult Notify(string isbnno)
        {
            var db = new BookEntities5();
            Request req = new Request();
            Book book = new Book();
            book = db.Books.Where(item => item.ISBNNO == isbnno).FirstOrDefault();
            req.email = Convert.ToString(Session["id"]);
            req.author = book.author_name;
            req.book_name = book.name;
            req.publisher = book.publisher;
            db.Requests.Add(req);
            db.SaveChanges();
            TempData["notify"] = "you will be notified when the book is available";
            return RedirectToAction("Display");
        }

        [HttpGet]
        public ActionResult History()
        {
            var db = new BookEntities5();
            string email = Convert.ToString(Session["id"]);
            user user = new user();
            user = db.users.Where(item => item.email == email).FirstOrDefault();
            List<Borrow> list = db.Borrows.Where(item => item.user_id == user.id).OrderBy(item => item.issue_date).ToList();
            return View(list);
        }
        public ActionResult Search()
        {
            ViewBag.count = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Search(string author_name, string name, string publisher)
        {
            var pageValue = Request.Form["page"];
            if (pageValue == null)
            {
                ViewBag.page = 1;
                pageValue = "1";
            }
            else
            {
                ViewBag.page = pageValue;
            }

            if (Convert.ToInt32(pageValue) < 0)
            {
                ViewBag.page = 1;
            }
            List<Book> List = new List<Book>();
            if (author_name == "") author_name = null;
            if (name == "") name = null;
            if (publisher == "") publisher = null;

            using (var db = new BookEntities5())
            {

                var query = db.spSearchBookModel(author_name, name, publisher, Convert.ToInt32(pageValue), 10);
                foreach (var item in query)
                {
                    List.Add(item as Book);
                }


                return View(List);
            }
        }
        [HttpGet]
        public ActionResult Display(int? pageNumber)
        {
            var db = new BookEntities5();
            var list = db.Books.ToList();
            return View(list.ToPagedList(pageNumber ?? 1, 10));
        }
    }
}