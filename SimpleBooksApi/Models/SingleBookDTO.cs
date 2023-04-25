using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBooksApi.Models
{
    public class SingleBookDTO
    {
       
            public int id { get; set; }
            public string name { get; set; }
            public string author { get; set; }
            public string isbn { get; set; }
            public string type { get; set; }
            public double price { get; set; }

            [JsonProperty("current-stock")]
            public int currentstock { get; set; }
            public bool available { get; set; }
       
    }
}
