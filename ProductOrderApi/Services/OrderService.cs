using ProductOrderApi.Data.Entities;
using ProductOrderApi.Data.Models;
using ProductOrderApi.Data.Repositories;

namespace ProductOrderApi.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;

        public OrderService(OrderRepository orderRepository, ProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await _orderRepository.GetOrdersAsync();
        }
        public async Task<Order?> GetOrder(int id)
        {
            return await _orderRepository.GetOrderAsync(id);
        }
        public async Task<Order> CreateOrder(CreateOrderModel newOrder)
        {
            // Create New Order Model
            Order order = new()
            {
                OrderDate = DateTime.Now,
            };

            decimal orderTotal = 0;
            for (int i = 0; i < newOrder.OrderProducts.Count - 1; i++) {
                OrderProductModel newOrderProduct = newOrder.OrderProducts[i];

                Product? product = _productRepository.GetProduct(newOrderProduct.ProductId).Result;
                if (product is null) {
                    continue;
                }

                OrderProduct orderProduct = new OrderProduct
                {
                    Product = product,
                    ProductId = product.Id,
                    Price = product.Price,

                    Order = order,
                    Quantity = newOrderProduct.Quantity
                };

                order.OrderProducts.Add(orderProduct);
                orderTotal += product.Price * newOrderProduct.Quantity;
            }

            // Calculate Order Total
            order.TotalPrice = orderTotal;

            // Save Order
            return await _orderRepository.CreateOrderAsync(order);
        }
        public async Task<Order?> UpdateOrder(Order order)
        {
            return await _orderRepository.UpdateOrderAsync(order);
        }
        public async Task DeleteOrder(int id)
        {
            await _orderRepository.DeleteOrderAsync(id);
        }
    }
}
