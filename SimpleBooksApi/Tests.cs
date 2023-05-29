using FluentAssertions;
using RestSharp;
using System.Net;
using SimpleBooksApi.Models;
using System.Text.Json;
using SimpleBooksApi.Utils;
using SimpleBooksApi.ReqestHelpers;


namespace SimpleBooksApi
{
    public class Tests : BaseActions
    {
        private RestClient _client;
        public Tests()
        {
            var restLibrary = new RestLibrary();
            _client = restLibrary.RestClient;
        }

        [Fact]
        public async Task GenerateTokenSucssesfuly()
        {
            var token = await GenerateAccessToken(Properties.authPath);

            //Assertion
            token.Should().NotBeNull();
            token.Should().BeOfType<string>();
        }

        [Fact]
        public async Task GetApiStatus()
        {
            //Create Request
            var request = new RestRequest("/status");

            //Execute GET operation
            var response = await _client.GetAsync<ApiStatusDTO>(request);

            //Assert
            response?.Status.Should().Be("OK");
        }

        [Fact]
        public async Task GetListOfBooks()
        {
            //Create Request
            var request = new RestRequest("/books");

            //Execute GET Operation
            var response = await _client.GetAsync(request);
            var booksList = JsonSerializer.Deserialize<List<BooksDTO>>(response.Content.ToString());

            //Assertion
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            booksList?.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetListOfFictionBooks()
        {
            //Create Request
            var request = new RestRequest("/books");
            request.AddQueryParameter("type", "fiction");

            //Execute GET operation
            var response = await _client.ExecuteAsync<List<BooksDTO>>(request);

            //Assertion
            foreach (var book in response.Data)
            {
                book.type.Should().Be("fiction");
            }

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetBooksByGivenLimitNumber()
        {
            //Create RestRequest
            var request = new RestRequest("/books");
            request.AddQueryParameter("limit", "3");

            //Execute GET Operation
            var response = await _client.ExecuteAsync<List<BooksDTO>>(request);
            var booksResponse = JsonSerializer.Deserialize<List<BooksDTO>>(response.Content.ToString());

            //Assertions
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            booksResponse?.Count.Should().Be(3);
        }

        [Fact]
        public async Task GetBooksByLimitAndTypeFiction()
        {
            //Create RestRequest
            var request = new RestRequest("/books");
            request.AddQueryParameter("type", "fiction");
            request.AddQueryParameter("limit", "3");

            //Execute GET Operation
            var response = await _client.ExecuteAsync<List<BooksDTO>>(request);

            //Assertions
            foreach (var book in response.Data)
            {
                book.type.Should().Be("fiction");
            }

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetBooksByLimitAndTypeNonFiction()
        {
            //Create RestRequest
            var request = new RestRequest("/books");
            request.AddQueryParameter("type", "non-fiction");
            request.AddQueryParameter("limit", "3");

            //Execute GET Operation
            var response = await _client.ExecuteAsync<List<BooksDTO>>(request);

            //Assertions
            foreach (var book in response.Data)
            {
                book.type.Should().Be("non-fiction");
            }

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task TestBookLimitValidation()
        {
            //This endpoint has Bug and 0 is still valid case and returns list of objects
            //Thats why the check is for less than 0 and bigger than 20

            // Create Random Number to validate that the functionality is working as expected
            Random random = new Random();
            int randomLimit = random.Next(-1, 21);

            // Create RestRequest
            var request = new RestRequest("/books");
            request.AddQueryParameter("limit", randomLimit.ToString());

            // Execute GET Operation
            var response = await _client.ExecuteAsync(request);

            if (randomLimit >= 0 && randomLimit <= 20)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorsDTO>(response.Content.ToString());

                if (randomLimit < 0)
                {
                    errorResponse.error.Should().Be("Invalid value for query parameter 'limit'. Must be greater than 0.");
                }
                else if (randomLimit > 20)
                {
                    errorResponse.error.Should().Be("Invalid value for query parameter 'limit'. Cannot be greater than 20.");
                }

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task GetBookById()
        {
            //Create RestRequest
            var request = new RestRequest("/books");
            request.AddUrlSegment("id", "1");

            //Performe GET Operation
            var response = await _client.ExecuteAsync(request);

            //Assertion
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var book = JsonSerializer.Deserialize<List<SingleBookDTO>>(response.Content);

            Assert.Equal("The Russian", actual: book[0].name);
        }

        [Fact]
        public async Task SubmitOrder()
        {
            var token = await GenerateAccessToken(Properties.authPath);

            //Create Request
            var request = new RestRequest("/orders");

            // Request Body
            var newOrder = new NewOrderDTO
            {
                bookId = 1,
                customerName = "Jake"
            };

            request.AddHeader(Properties.AUTHORIZATION, token);
            request.AddBody(newOrder);

            //Performe POST Operation
            var response = await _client.PostAsync(request);
            OrderDTO order = JsonSerializer.Deserialize<OrderDTO>(response.Content.ToString());

            //Assererions
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            order.created.Should().BeTrue();
            order.orderId.Should().NotBeNull();
            order.orderId.Should().BeOfType<string>();
        }

        [Fact]
        public async Task SubmitNewOrderWithMissingAuthorization()
        {
            var request = new RestRequest("/orders");
            var newOrder = new NewOrderDTO
            {
                bookId = 1,
                customerName = "Jake"
            };
            request.AddJsonBody(newOrder);

            //Execute Post Operation
            var response = await _client.ExecutePostAsync<ErrorsDTO>(request);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            // Assert that the error message is correct
            var errorResponse = JsonSerializer.Deserialize<ErrorsDTO>(response.Content);
            errorResponse?.error.Should().Be("Missing Authorization header.");
        }

        [Fact]
        public async Task SubmitNewOrderWithInvalidId()
        {
            var token = await GenerateAccessToken(Properties.authPath);

            //Create RestRequest
            var request = new RestRequest("/orders");
            request.AddHeader(Properties.AUTHORIZATION, token);
            request.AddJsonBody(new NewOrderDTO
            {
                bookId = 0,
                customerName = "Jake Ozie"
            });

            var response = await _client.ExecutePostAsync<ErrorsDTO>(request);

            //Assertions
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorResponse = JsonSerializer.Deserialize<ErrorsDTO>(response.Content);
            errorResponse.error.Should().Be("Invalid or missing bookId.");
        }

        [Fact]
        public async Task GetAllOrders()
        { 
            //Generate Access Token
            var token = await GenerateAccessToken(Properties.authPath);

            //Create RestRequest
            var request = new RestRequest("/orders");
            request.AddHeader(Properties.AUTHORIZATION, token);

            var response = await _client.ExecuteGetAsync<List<CreatedOrdersDTO>>(request);

            foreach (var orders in response.Data)
            {
                orders.id.Should().NotBeEmpty();
                orders.bookId.Should().NotBeEmpty();
                orders.quantity.Should().BeGreaterThan(0);
            }

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateAnOrder()
        {
            //this is the OrderId that will be updated
            string orderId = await CreateNewOrder();

            //Generate Access Token
            var token = await GenerateAccessToken(Properties.authPath);

            //Create RestRequest
            var request = new RestRequest($"/orders/{orderId}");

            request.AddHeader(Properties.AUTHORIZATION, token);
            request.AddJsonBody(new NewOrderDTO 
            { 
                customerName =  "Updated Customer Name"
            });

            var response = await _client.PatchAsync(request);

            //Assertion
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteAnOrder()
        {
            //this is the OrderId that will be deleted
            string orderId = await CreateNewOrder();

            //Generate Access Token
            var token = await GenerateAccessToken(Properties.authPath);

            //Create RestRequest
            var request = new RestRequest($"/orders/{orderId}");

            request.AddHeader(Properties.AUTHORIZATION, token);
            var response = await _client.DeleteAsync(request);

            //Assertion
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            //Check that the order no longer exists
            var message = await GetOrderById(orderId);
            message.Should().Be($"Order with ID {orderId} does not exist.");
        }

        private async Task<string> GetOrderById(string orderId)
        {
            //Generate Access Token
            var token = await GenerateAccessToken(Properties.authPath);

            //Create RestRequest
            var request = new RestRequest($"/orders/{orderId}");

            //Add authorization header
            request.AddHeader(Properties.AUTHORIZATION, token);

            //Make GET request
            var response = await _client.GetAsync(request);

            //Check if order exists and return message
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return $"Order with ID {orderId} exists.";
            }
            else
            {
                return $"Order with ID {orderId} does not exist.";
            }
        }

        private async Task<string> CreateNewOrder()
        {
            //Generate Access Token
            var token = await GenerateAccessToken(Properties.authPath);

            //Prerequesite for this test is to create New Order
            var createOrderRequest = new RestRequest("/orders");

            // Request Body
            var newOrder = new NewOrderDTO
            {
                bookId = 1,
                customerName = "Jake"
            };

            createOrderRequest.AddHeader(Properties.AUTHORIZATION, token);
            createOrderRequest.AddBody(newOrder);

            //Performe POST Operation
            var createOrder = await _client.PostAsync(createOrderRequest);
            OrderDTO order = JsonSerializer.Deserialize<OrderDTO>(createOrder.Content.ToString());
            string orderId = order.orderId;

            return orderId;
        }
    }
}