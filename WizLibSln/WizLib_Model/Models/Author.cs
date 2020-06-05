using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WizLib_Model.Models
{
    public class Author
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Author_Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public DateTime BirthDay { get; set; }
        public string Location { get; set; }
        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        //public List<Book> Books { get; set; }
    }
}
