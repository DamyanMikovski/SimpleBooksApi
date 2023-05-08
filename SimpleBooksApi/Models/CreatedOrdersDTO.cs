namespace SimpleBooksApi.Models
{
    public class CreatedOrdersDTO
    {
        public string id { get; set; }
        public string bookId { get; set; }
        public string customerName { get; set; }
        public string createdBy { get; set; }
        public int quantity { get; set; }
        public string timestamp { get; set; }
    }
}
