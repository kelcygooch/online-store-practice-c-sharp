using System;
namespace ProductOrderApi.Data.Entities
{
    public class Order
    {
        public int Id { get; set; } = 0; 
        public decimal TotalPrice { get; set; } = 0; 
        public DateTime OrderDate { get; set; } = new DateTime();
        public virtual List<OrderProduct?> OrderProducts { get; set; } = new List<OrderProduct?>();
    }
}
