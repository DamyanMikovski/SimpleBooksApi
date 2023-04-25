using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBooksApi.Models
{
    public class BooksDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool available  { get; set; }
    }
}
