using MiniProject.Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniProject.Library.Controllers
{
    public class ReturnBooksController : Controller
    {
        // GET: ReturnBooks
        public ActionResult ReturnDisplay()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReturnDisplay(Borrow borrow)
        {
            Session["returnid"] = borrow.user_id;
            return RedirectToAction("Return");
        }

        [HttpGet]
        public ActionResult Return()
        {
            var db = new BookEntities5();
            int id = Convert.ToInt32(Session["returnid"]);
            List<Borrow> list = db.Borrows.Where(item => item.user_id == id&&item.returned=="N ").ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult ReturnAction(int id)
        {
            var db = new BookEntities5();
            Borrow borrow = new Borrow();
            borrow = db.Borrows.Where(item => item.copy_id == id).FirstOrDefault();
            borrow.return_date = DateTime.Today;
            borrow.returned = "Y";
            
            Copy copies = new Copy();
            copies = db.Copies.Where(item => item.copy_id == id).FirstOrDefault();
            string isbnno = copies.ISBNNO;
            copies.copies_available = "Y";
            int flag = 0;
            Book book = new Book();
            book = db.Books.Where(item => item.ISBNNO == isbnno).FirstOrDefault();
            if(book.copies<=0)
            {
                flag = 1;
            }
            book.copies = book.copies + 1;
            if(book.status!="available")
            {
                book.status = "available";
            }
            db.SaveChanges();
            if(flag==1)
            {
                Notification notification = new Notification();
                notification.notify(book);
            }
            TempData["returned"] = "Book Returned";
            return RedirectToAction("Return");

        }
    }
}