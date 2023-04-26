using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBooksApi.Utils
{
    public class Properties
    {
        public const string booksBaseUrl = "https://simple-books-api.glitch.me";
        public const string authPath = "https://simple-books-api.glitch.me/api-clients/";

        //Generating random usernam and random Email
        public static string userCredentials = "{\"clientName\":\"" + Guid.NewGuid().ToString() + "\",\"clientEmail\":\"" + Guid.NewGuid().ToString() + "@example.com\"}";
        public static string clientName =  Guid.NewGuid().ToString();
        public static string clientEmail = Guid.NewGuid().ToString() + "@example.com";
        public const string BEARER_TOKEN = "Bearer";
    }
}
