using RestSharp;
using SimpleBooksApi.Utils;
using SimpleBooksApi.Models;
using Newtonsoft.Json;

namespace SimpleBooksApi.ReqestHelpers
{
    public class BaseActions
    {
        private static RestClient restClient;
        private static string url;

        public async Task<string> GenerateAccessToken(string url)
        { 
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader(Properties.userCredentials, DataFormat.Json);

            var response = await restClient.GetAsync(request);
            TokenDTO token = JsonConvert.DeserializeObject<TokenDTO>(response.Content.ToString());

            string accessToken = token.AccessToken;

            return accessToken;
        }
    }
}
