using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities.Order_Aggregate;
using ECommerce.Core.Entities.Product;
using ECommerce.Core.Reposiroties_Contracts;
using ECommerce.Core.Services_Contracts;
using ECommerce.Core.Specification;
using ECommerce.Repository.Data;

namespace ECommerce.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        private readonly StoreContext _dbContext;

        public OrderService(
            IBasketRepository basketRepository,
            IGenericRepository<Product> productRepo,
            IGenericRepository<Order> orderRepo,
            IGenericRepository<DeliveryMethod> deliveryMethodRepo,
            StoreContext dbContext)
        {
            _basketRepository = basketRepository;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _deliveryMethodRepo = deliveryMethodRepo;
            _dbContext = dbContext;
        }

        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null) return null;

            var orderItems = new List<OrderItem>();

            if (basket.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.Id);
                    if (product != null)
                    {
                        var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                        var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                        orderItems.Add(orderItem);
                    }
                }
            }

            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            var deliveryMethod = await _deliveryMethodRepo.GetByIdAsync(deliveryMethodId);

            var order = new Order
            (
                buyerEmail: buyerEmail,
                shippingAddress: shippingAddress,
                deliveryMethodId: deliveryMethodId,
                items: orderItems,
                subtotal: subtotal
            );

            _orderRepo.Add(order);
            await _dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecification(buyerEmail);
            var orders = await _orderRepo.GetAllWithSpecAsync(spec);
            return orders.ToList();
        }

        public async Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecification(orderId, buyerEmail);
            var order = await _orderRepo.GetByIdWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await _deliveryMethodRepo.GetAllAsync();
            return deliveryMethods.ToList();
        }
    }
}

