using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ProductOrderApi.Data.Entities;
using ProductOrderApi.Data.Models;
using ProductOrderApi.Data.Repositories;
using ProductOrderApi.Helpers;
using ProductOrderApi.Services;

namespace ProductOrderApi.Tests 
{
    public class ServiceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DataHelper _helper;
        private readonly ProductService _productService;
        private readonly OrderService _orderService; 

        public ServiceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var scope = _factory.Services.CreateScope();
            _helper = scope.ServiceProvider.GetRequiredService<DataHelper>();
            _helper.SeedData();

            var productRepository = scope.ServiceProvider.GetRequiredService<ProductRepository>();
            var orderRepository = scope.ServiceProvider.GetRequiredService<OrderRepository>();

            _productService = new ProductService(productRepository);
            _orderService = new OrderService(orderRepository, productRepository);
        }
        
        [Fact]
        public async Task GetProducts_ReturnsProducts()
        {
            var products  = await _productService.GetProducts();
            foreach (Product product in products) {
                Assert.NotEqual(0, product.Id);
            }
        }

        [Fact]
        public async Task GetProduct_ReturnsProduct()
        {   var product = _helper.GetProduct() ?? throw new Exception("No Product Returned");

            var serviceProduct = await _productService.GetProduct(product.Id);

            Assert.NotNull(serviceProduct); 
            Assert.Equal(product.Id, serviceProduct.Id);
            Assert.Equal(product.Name, serviceProduct.Name);
            Assert.Equal(product.Price, serviceProduct.Price);
        }

        [Fact]
        public async Task GetProductsByIds_ReturnsProduct()
        {
            var products = _helper.GetProducts(3); 
            var ids = new List<int>();

            foreach (Product p in products) {
                ids.Add(p.Id);
            }

            var serviceProducts = await _productService.GetProductsByIds(ids);
            foreach (Product p in serviceProducts) {
                Assert.Contains(p.Id, ids);
            }
        }

        [Fact]
        public async Task AddProduct_ReturnsProduct()
        {
            Product newProduct = new()
            {
                Id = 8000,
                Name = "New Unit Test Product",
                Price = 8.45m,
            };

            var product = await _productService.AddProduct(newProduct);
            
            Assert.NotNull(product); 
            Assert.Equal(product.Id, newProduct.Id);
            Assert.Equal(product.Name, newProduct.Name);
            Assert.Equal(product.Price, newProduct.Price);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsUpdatedProduct()
        {

            Product p = _helper.GetProduct()?? throw new Exception("No Product Returned");
            var oldPrice = p.Price; 
            p.Price += 4;

            var updatedProduct = await _productService.UpdateProduct(p) ?? throw new Exception("No Product Returned");
            Assert.NotNull(updatedProduct); 
            Assert.Equal(p.Id, updatedProduct.Id);
            Assert.Equal(p.Name, updatedProduct.Name);
            Assert.NotEqual(oldPrice, updatedProduct.Price);

            Assert.Equal(oldPrice + 4, updatedProduct.Price);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNullProduct()
        {
            Product p = new() {
                Id = 1234,
            };

            var updatedProduct = await _productService.UpdateProduct(p);
            Assert.Null(updatedProduct); 
        }

        [Fact]
        public async Task DeleteProduct_ProductDeletedTrue()
        {
            Product p = _helper.GetProduct()?? throw new Exception("No Product Returned");
            Assert.True(await _productService.DeleteProduct(p.Id));
        }
        
        [Fact]
        public async Task DeleteProduct_ProductDeletedFalse()
        {
            Product p = new(){
                Id = 123456789
            };
            Assert.True(await _productService.DeleteProduct(p.Id));

        }

        [Fact]
        public async Task GetOrders_ReturnsOrders()
        {
           var orders = await _orderService.GetOrders();
           Assert.NotEmpty(orders);
        }

        [Fact]
        public async Task GetOrder_ReturnsOrderById()
        {
            var order = _helper.GetOrder()?? throw new Exception("No Order Returned");
            var serviceOrder = await _orderService.GetOrder(order.Id);
            Assert.NotNull(serviceOrder);
            Assert.Equal(order.Id, serviceOrder.Id);
            Assert.Equal(order.OrderDate, serviceOrder.OrderDate);
        }

        // [Fact]
        // public async Task CreateOrder_OrderCreatedAndReturned() {}

        // [Fact]
        // public async Task UpdateOrder() {}

        // [Fact]
        // public async Task DeleteOrder(){}
    }
}