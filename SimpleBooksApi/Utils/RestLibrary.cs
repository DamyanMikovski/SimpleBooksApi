using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBooksApi.Utils
{
    public class RestLibrary
    {
        public RestLibrary()
        {
            var restClientOptions = new RestClientOptions
            {
                BaseUrl = new Uri("https://simple-books-api.glitch.me")
            };

            //Rest Client
            RestClient = new RestClient(restClientOptions);
        }

        public RestClient RestClient { get; }
    }
}
