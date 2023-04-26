using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBooksApi.Models
{
    public class BookOrderDTO
    {
        public int bookId { get; set; }
        public string customerName { get; set; }
    }
}
