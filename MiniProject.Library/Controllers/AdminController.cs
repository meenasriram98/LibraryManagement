using MiniProject.Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace MiniProject.Library.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult AdminHome()
        {
            return View();
        }
        // GET: Admin

        public ActionResult AddBooks()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddBooks(Book book)
        {
            if (ModelState.IsValid)
            {
                string str = book.ISBNNO.Trim();
                if (str.Contains(" "))
                {
                    ViewBag.InvalidIsbn = "entered isbn is not valid";
                    return View();
                }
                var db = new BookEntities5();
                Book b = new Book();
                Copy copies = new Copy()
                {
                    ISBNNO = book.ISBNNO,
                    copies_available = "Y"
                };
                int count = 0;
                b = db.Books.Where(item => item.ISBNNO == book.ISBNNO).FirstOrDefault();
                if (b == null)
                {
                    book.status = "available";
                    book.department = "cse";
                    book.books_date = DateTime.Today;
                    //b.subject = "c";
                    db.Books.Add(book);
                    ViewBag.success = "Book Added Successfully";
                    while (count < book.copies)
                    {
                        db.Copies.Add(copies);
                        db.SaveChanges();
                        count++;
                    }
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.error = "ISBN already exixts";
                    return View();
                }
            }
            return View();
        
    }
        [HttpGet]
        public ActionResult Display(int? pageNumber)
        {
            var db = new BookEntities5();
            var list=db.Books.ToList();
            return View(list.ToPagedList(pageNumber??1,10));
        }

        [HttpGet]
        public ActionResult Add(string id)
        {
            ViewBag.Isbn = id;
            Session["isbn"] = id;
            return View();
        }

        [HttpPost]
        public ActionResult Add(Book book)
        {
            string isbn = Convert.ToString(Session["isbn"]);
            Copy copies = new Copy
            {
                ISBNNO = isbn,
                copies_available = "Y"
            };
            int count = 0, flag = 0; ;
            Book b = new Book();
            var db = new BookEntities5();
            b = db.Books.Where(item => item.ISBNNO == isbn).FirstOrDefault();
            if(b.copies<=0)
            {
                flag = 1;
            }
            b.copies = b.copies+book.copies;
            string isbnno = b.ISBNNO;
            db.SaveChanges();
            while(count<book.copies)
            {
                db.Copies.Add(copies);
                db.SaveChanges();
                count++;
            }
            db.SaveChanges();
            TempData["CopiesAdded"] = "Copies Added Successfully";
            if (flag == 1)
            {
                Notification notification = new Notification();
                notification.notify(b);
            }
            return RedirectToAction("CopyDetail", "Admin",new { id = isbnno });
        }

        [HttpGet]
        public ActionResult CopyDetail(string id)
        {
            if(id==null)
            {
                return View("CopyDetail1");
            }

            var db = new BookEntities5();
            List<Copy> list = db.Copies.Where(item => item.ISBNNO == id).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult DeleteCopy(int copy_id)
        {
            var db = new BookEntities5();
            Borrow borrowed = new Borrow();
            Copy book_copy = new Copy();
            Book b = new Book();
            Borrow history = new Borrow();
            
            book_copy = db.Copies.Where(item => item.copy_id == copy_id).FirstOrDefault();
            borrowed = db.Borrows.Where(item => item.copy_id == book_copy.copy_id&&item.returned=="N ").FirstOrDefault();
            string isbnno = book_copy.ISBNNO;
            history = db.Borrows.Where(item => item.ISBNNO == isbnno && item.returned == "N").FirstOrDefault();

            List<Borrow> delete = db.Borrows.Where(item => item.copy_id == copy_id).ToList();
            b = db.Books.Where(item => item.ISBNNO == isbnno).FirstOrDefault();
            if (borrowed != null && borrowed.returned == "N ")
            {
                TempData["error"] = "book has not been returned so copy cannot be deleted";
                //return RedirectToAction("Display");
                return RedirectToAction("copyDetail", "Admin", new { id = isbnno });
            }
            //if (borrowed != null)
            //{
            //    db.Borrows.Remove(borrowed);

            //}
            foreach (var item in delete)
            {
                db.Borrows.Remove(item);
                db.SaveChanges();
            }
            db.Copies.Remove(book_copy);
            TempData["CopyRemoved"] = "Copy Removed Successfully";
            db.SaveChanges();
            b.copies = b.copies - 1;
            if(b.copies<=0)
            {
                b.status = "unavailable";
            }
            if(b.copies<=0&&history==null)
            {
                b.status = "unavailable";
                db.Books.Remove(b);
            }
            db.SaveChanges();
            string id = isbnno;
            return RedirectToAction("copyDetail","Admin",new { id = isbnno });


        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            //add a pop up for error message
            var db = new BookEntities5();
            
            Book b = new Book();
            b = db.Books.Where(item => item.ISBNNO == id).FirstOrDefault();
            List<Copy> copies = db.Copies.Where(item => item.ISBNNO == id).ToList();
            List<Borrow> borrowlist = db.Borrows.Where(item => item.ISBNNO == id).ToList();
            foreach (Borrow temp in borrowlist)
            {
                //db.Borrows.Remove(temp);
                if (temp.returned != "N ")
                {
                    db.Borrows.Remove(temp);
                }
                else
                {
                    TempData["delete"] = "cannot be deleted";
                    TempData["DeleteError"] = "all copies haven't been returned the book cannot be deleted";
                    return RedirectToAction("Display");
                }
            }
            foreach (Copy temp in copies)
            {
                db.Copies.Remove(temp);
            }
            try
            {
                db.Books.Remove(b);
                db.SaveChanges();
                TempData["removed"] = "Book removed";
                return RedirectToAction("Display");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

        }

        [HttpGet]
        public ActionResult Lend()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Lend(Borrow borrow)
        {

            Account acc = new Account();
            Book b = new Book();
            var db = new BookEntities5();
            acc = db.Accounts.Where(item => item.id == borrow.user_id).FirstOrDefault();
            List<Copy> copies = db.Copies.Where(item => item.ISBNNO == borrow.ISBNNO).ToList();
            string isbn = copies[0].ISBNNO;
            b = db.Books.Where(item => item.ISBNNO == isbn).FirstOrDefault();
            b.copies = b.copies - 1;
            if(b.copies<=0)
            {
                b.status = "unavailable";
            }
            var book_id = isbn;
            if (acc == null)
            {
                ViewBag.ErrorMessage = "account not found";
                return View();
            }
            else if (acc.books_borrowed == 5)
            {
                ViewBag.ErrorMessage = "user has already borrowed 5 books";
            }
            else
            {
                List<Borrow> borrowed = db.Borrows.Where(item => item.user_id == borrow.user_id).ToList();
                foreach (Borrow Temp in borrowed)
                {
                    if (Temp.ISBNNO == book_id)
                    {
                        ViewBag.ErrorMessage = "user has already borrowed the same book";
                        return View();
                    }
                }
                foreach(Copy item in copies)
                {
                    if(item.copies_available=="Y")
                    {
                        borrow.copy_id = item.copy_id;
                        item.copies_available = "N";
                        break;
                    }
                }
                borrow.ISBNNO = book_id;
                borrow.borrow1 = "Y";
                borrow.issue_date = DateTime.Today;
                borrow.return_date = borrow.issue_date.AddDays(5);
                borrow.returned = "N";
                db.Borrows.Add(borrow);
                db.SaveChanges();
                ViewBag.SuccessMessage = "book issued successfully";
            }
            return View();
        }

        //public ActionResult SearchBar(string search)
        //{
        //    BookEntities5 db = new BookEntities5();
        //    return View(db.Books.Where(x => x.name.Contains(search) || x.subject.Contains(search) || x.publisher.Contains(search) || x.author_name.Contains(search) || x.department.Contains(search) || search == null).ToList());

        //}

        [HttpGet]
        public ActionResult EditBook(string id)
        {
            var db = new BookEntities5();
            var bookId = db.Books.Find(id);
            if (bookId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {

                return View(bookId);
            }

        }

        [HttpPost]
        public ActionResult EditBook(Book book)
        {
            try
            {
                var db = new BookEntities5();
                Book b = new Book();
                b = db.Books.Find(book.ISBNNO);
                b.name = book.name;
                b.publisher = book.publisher;
                b.subject = book.subject;
                b.status = book.status;
                b.department = book.department;
                b.copies = book.copies;
                b.author_name = book.author_name;
                db.SaveChanges();
                return RedirectToAction("Display");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        [HttpGet]
        public ActionResult NewlyAddedBooks()
        {
            var db = new BookEntities5();
            return View(db.Books.OrderBy(item => item.books_date).Take(4).ToList());

        }

       

        [HttpGet]
        public ActionResult UserProfile()
        {
            var db = new BookEntities5();
            Admin user1 = new Admin();
            string email = Convert.ToString(Session["id"]);
            user1 = db.Admins.Where(item => item.email == email).FirstOrDefault();
            

            return View(user1);
        }



        public ActionResult Search()
        {
            ViewBag.count = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Search(string author_name, string name, string publisher)
        {

            //var searchString = "";
            //if (Request.Form["prevclicked"] != null) searchString = Request.Form["prevclicked"];
            //else if (Request.Form["nextclicked"] != null) searchString = Request.Form["nextclicked"];
            //string[] str = searchString.Split(',');
            //if (name == null && author_name == null && publisher == null)
            //{
            //    name = str[0];
            //    author_name = str[1];
            //    publisher = str[2];
            //}
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



    }
}


    
