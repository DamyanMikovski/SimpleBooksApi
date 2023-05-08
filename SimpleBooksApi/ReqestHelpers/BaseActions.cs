using RestSharp;
using SimpleBooksApi.Utils;
using SimpleBooksApi.Models;
using Newtonsoft.Json;

namespace SimpleBooksApi.ReqestHelpers
{
    public class BaseActions
    {
        public static string accessToken = string.Empty;

        public async Task<string> GenerateAccessToken(string url)
        {
            if (accessToken == string.Empty)
            {

                var client = new RestClient(url);
                var request = new RestRequest(url);
                request.AddHeader("Content-Type", "application/json");
                var user = new CredentialsDTO
                {
                    clientName = Properties.clientName,
                    clientEmail = Properties.clientEmail
                };
                
                request.AddBody(user);

                var response = await client.PostAsync(request);
                AccessTokenDTO token = JsonConvert.DeserializeObject<AccessTokenDTO>(response.Content.ToString());

                accessToken = token.accessToken;
            }

            return accessToken;
        }
    }
}
