using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniProject.Library.ViewModels
{
    public class Registration
    {

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id required")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        public string name { get; set; }
        [Display(Name = "phoneNumber")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "number is required")]
        public string number { get; set; }
        [Display(Name = "Department")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "department is required")]
        public string department { get; set; }
        [Display(Name = "User Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "type is required")]
        public string type { get; set; }
        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email id required")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}