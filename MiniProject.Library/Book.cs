//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiniProject.Library
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class Book
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Book()
        {
            this.Borrows = new HashSet<Borrow>();
            this.Copies1 = new HashSet<Copy>();
        }


        [DisplayName("ISBNNO *")]
        public string ISBNNO  { get; set; }
        [DisplayName("Book Name *")]
        public string name { get; set; }
        [DisplayName("Department *")]
        public string department { get; set; }
        [DisplayName("Subject *")]
        public string subject { get; set; }
        [DisplayName("Status")]
        public string status { get; set; }
        [DisplayName("Author Name *")]
        public string author_name { get; set; }
        [DisplayName("Publisher *")]
        public string publisher { get; set; }
        [DisplayName("Copies *")]
        public int copies { get; set; }
        [DisplayName("Date *")]
        public Nullable<System.DateTime> books_date { get; set; }
        [DisplayName("Year Of Publication *")]
        public Nullable<int> YOP { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Borrow> Borrows { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Copy> Copies1 { get; set; }
    }
}
