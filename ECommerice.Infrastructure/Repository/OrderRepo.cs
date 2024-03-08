using ECommerice.Core.Entities;
using ECommerice.Core.Entities.OrderAggregate;
using ECommerice.Core.IRepository;
using ECommerice.Core.IUniteOfWork;
using ECommerice.Core.Specification;
using ECommerice.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Infrastructure.Repository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly IUniteOfWork _uniteOfWork; 
        private readonly IMailService _mailService; 

        public OrderRepo(IUniteOfWork uniteOfWork, IMailService mailService)
        {
            _uniteOfWork = uniteOfWork;
            _mailService = mailService;
        }

        public async Task<IEnumerable<Order>> GetOrdersSearch(int pageNo,int pageSize,DateTime? dateFrom, DateTime? dateTo)
        {
            var query = await _uniteOfWork.Repository<Order>()
                    .GetWhere(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo);

            return query;
        }

        //public async Task<int> GetOrdersSearchCount(DateTime? dateFrom, DateTime? dateTo)
        //{
        //    var query = await _uniteOfWork.Repository<Order>()
        //        .GetWhere(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo);
        //    return query.Count;
        //}

        public async Task<Order> CreateOrderAsync(string userId, string userName, List<OrderItem> OrderItems)
        {
            //calc subtotal
            var subTotal = OrderItems.Sum(item => item.Price * item.Quantity - item.Discount);

            //create order
            var order = new Order();
            order.OrderDate = DateTime.Now;
            order.UserId = userId;
            order.Username = userName;
            order.TotalAmount = subTotal;
            
            foreach (var item in OrderItems)            
                item.Total = (item.Price * item.Quantity) - item.Discount;            

            order.OrderItems = OrderItems;
            _uniteOfWork.Repository<Order>().Add(order);

            //foreach (var item in OrderItems)
            //{
            //    var product = await _uniteOfWork.Repository<Product>().GetWhereObject(p => p.Id == item.ProductId);
            //    product.Quantity = product.Quantity - item.Quantity;
            //    product.Price = product.Price - item.Discount;

            //    _uniteOfWork.Repository<Product>().Update(product);
            //}

            //save in db
            var result = await _uniteOfWork.CompleteAsync();
            if (result <= 0) return null;


            //send mail

            await _mailService.SendEmail("", "", "", "");

            //retun order
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _uniteOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _uniteOfWork.Repository<Order>().ListAllAsync();
        }
        
        public async Task<IEnumerable<Order>> GetOrderForUserAsync(string userId)
        {
            var query = await _uniteOfWork.Repository<Order>().GetWhere(o => o.UserId == userId);
            return query;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItems(int orderId)
        {
            var query = await _uniteOfWork.Repository<OrderItem>()
                .GetWhere(o => o.OrderId == orderId);
            return query;
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            var query = await _uniteOfWork.Repository<Order>().GetWhereObject(o => o.Id == orderId);
            return query;
        }
    }
}
