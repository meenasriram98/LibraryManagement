using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MiniProject.Library.ViewModels
{
    public class Notification
    {
        public void notify(Book b)
        {
            var db = new BookEntities5();
            List<Request> list = db.Requests.Where(item => item.book_name == b.name && item.author==b.author_name && item.publisher==b.publisher).ToList();
            foreach (var item in list)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("s.meena9874@gmail.com"));
                message.From = new MailAddress("nikhitha.devidi@imaginea.com");
                message.Subject = "notification about availability";
             
                message.Body = string.Format(body, "nikhitha", "nikhitha.devidi@imaginea.com", "To return the book in 1 day");
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "nikhitha.devidi@imaginea.com",
                        Password = "Pramati@123"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(message);

                }
            }

        }
    }
}