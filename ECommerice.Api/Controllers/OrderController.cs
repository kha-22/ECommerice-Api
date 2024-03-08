using AutoMapper;
using ECommerice.Api.DTOs;
using ECommerice.Api.Helpers;
using ECommerice.Api.Helpers.Extensions;
using ECommerice.Core.Entities;
using ECommerice.Core.Entities.OrderAggregate;
using ECommerice.Core.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : BaseApiController
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IGenericRepository<Order> _gOrderRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(IOrderRepo orderRepo, IMapper mapper,
            UserManager<AppUser> userManager,
            IGenericRepository<Order> gOrderRepo)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _userManager = userManager;
            _gOrderRepo = gOrderRepo;
        }

        [HttpPost("createOrder")]
        public async Task<ActionResult<Order>> CreateOrder(Order orderDTO)
        {
            var email = HttpContext.User?.RetrieveEmailFromClaimPrincipal();
            var user = await _userManager.FindByEmailAsync(email);
            var order = await _orderRepo.CreateOrderAsync(user.Id, user.DisplayName, orderDTO.OrderItems);

            if (order == null) return BadRequest(new ApiResponse(400, "Error in create order"));

            return Ok(order);
        }

        [HttpGet("getOrdersForCurrentUser")]
        public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetOrderForUser()
        {
            var email = HttpContext.User?.RetrieveEmailFromClaimPrincipal();
            var user = await _userManager.FindByEmailAsync(email);

            var orders = await _orderRepo.GetOrderForUserAsync(user.Id);
            return Ok(orders);
        }

        [HttpGet("getOrderDetails/{id}")]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrderDetails(int id)
        {
            var order = await _orderRepo.GetOrderById(id);
            var orderItems = await _orderRepo.GetOrderItems(id);

            var orderToReturn = new OrderToReturnDTO()
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Username = order.Username,
                TotalAmount = order.TotalAmount,
                OrderItems = orderItems
            };

            if (orderToReturn == null) return NotFound(new ApiResponse(404));

            return Ok(orderToReturn);
        }

        [HttpGet("getDeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
        {
            var result = await _orderRepo.GetDeliveryMethodsAsync();
            return Ok(result);
        }

        #region Admin only ======================================================
        [Authorize]
        [HttpPost("getOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders(OrderSearchDTO orderSearch)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            if(orderSearch.DateFrom == null || orderSearch.DateFrom == null)
            {
                var orders = await _gOrderRepo.GetAllPaging();
                return Ok(new
                {
                    Data = orders.Skip((orderSearch.PageNo - 1) * orderSearch.pageSize).Take(orderSearch.pageSize),
                    Count = orders.Count()
                });
            }
            else
            {
                var orders = await _orderRepo
                    .GetOrdersSearch(orderSearch.PageNo, orderSearch.pageSize, orderSearch.DateFrom, orderSearch.DateTo);
                return Ok(new
                {
                    Data = orders.Skip((orderSearch.PageNo - 1) * orderSearch.pageSize).Take(orderSearch.pageSize),
                    Count = orders.Count()
                });
            }
           
        }

        [Authorize]
        [HttpGet("getOrderItems/{id}")]
        public async Task<ActionResult<OrderToReturnDTO>> GetOrderItems(int id)
        {
            var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
            if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

            var order = await _orderRepo.GetOrderById(id);
            var orderItems = await _orderRepo.GetOrderItems(id);

            var orderToReturn = new OrderToReturnDTO()
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Username = order.Username,
                TotalAmount = order.TotalAmount,
                OrderItems = orderItems
            };

            if (orderToReturn == null) return NotFound(new ApiResponse(404));

            return Ok(orderToReturn);
        }


        //[Authorize]
        //[HttpPost("getOrdersSearch")]
        //public async Task<ActionResult<IEnumerable<OrderToReturnDTO>>> GetOrdersSearch(OrderSearchDTO orderSearch)
        //{
        //    var user = await _userManager.FindByEmailFromClaimPrincipleAsync(HttpContext.User);
        //    if (user.UserType == "user") return Unauthorized(new ApiResponse(401));

        //    var orders = await _orderRepo
        //        .GetOrdersSearch(orderSearch.pageSize, orderSearch.pageSize,orderSearch.DateFrom, orderSearch.DateTo);
        //    return Ok(new
        //    {
        //        Data = orders,
        //        Count = orders.Count()
        //    });
        //}
        #endregion

    }
}
