using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WizLib_Model.Models
{
    public class BookAuthor
    {
        //[Key]
        public int Book_Id { get; set; }
        //[Key]
        public int Author_Id { get; set; }
        public Book Book { get; set; }
        public Author Author { get; set; }
    }
}
