using FluentAssertions;
using RestSharp;
using System.Net;
using SimpleBooksApi.Models;
using System.Text.Json;
using SimpleBooksApi.Utils;


namespace SimpleBooksApi
{
    public class Tests
    {
        [Fact]
        public async Task GetApiStatus()
        {
            //Create RestClient
            var client = new RestClient("https://simple-books-api.glitch.me");

            //Create Request
            var request = new RestRequest("/status");

            //Execute Get operation
            var response = await client.GetAsync<ApiStatusDTO>(request);

            //Assert
            response?.Status.Should().Be("OK");
        }

        [Fact]
        public async Task GetListOfBooks()
        {
            //Create RestClien
            var client = new RestClient(Properties.booksBaseUrl);

            //Create Request
            var request = new RestRequest("/books");

            //Execute Get Operation
            var response = await client.GetAsync(request);
            var booksList = JsonSerializer.Deserialize<List<BooksDTO>>(response.Content.ToString());

            //Assertion
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            booksList?.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetListOfFictionBooks()
        {
            //Create Client
            var client = new RestClient(Properties.booksBaseUrl);

            //Create Request
            var request = new RestRequest("/books");
            request.AddQueryParameter("type", "fiction");

            //Execute Get operation
            var response = await client.ExecuteAsync<List<BooksDTO>>(request);

            //Assertion
            foreach (var book in response.Data)
            {
                book.type.Should().Be("fiction");
            }

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void GetBookById()
        {
            //Create RestClient
            var client = new RestClient(Properties.booksBaseUrl);

            //Create RestRequest
            var request = new RestRequest("/books");
            request.AddUrlSegment("id", "1");

            //var response = await client.ExecuteAsync<BooksDTO>(request);

            //Performe Get Operation
            var getOperation = await client.GetAsync(request);
            var response = JsonSerializer.Deserialize<List<SingleBookDTO>>(getOperation.Content);

            //Assertion
            Assert.Equal(response[0].name, "The Russian");
        }
    }
}