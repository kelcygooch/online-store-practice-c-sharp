using System.Text.Json.Serialization;

namespace ProductOrderApi.Data.Entities
{
    public class OrderProduct
    {
        public int Id { get; set; } = 0;
        public int OrderId { get; set; } = 0; 
        public int ProductId { get; set; } = 0;
        public int Quantity { get; set; } = 0; 
        public decimal Price { get; set; } = 0;
        [JsonIgnore]
        public Order Order { get; set; } = new Order();
        public Product Product { get; set; } = new Product();
    }
}
