using ECommerice.Core.Entities;
using ECommerice.Core.IRepository;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Infrastructure.Repository
{
    public class ProductRepo : IProductRepo
    {
        public readonly StoreContext _context;

        public ProductRepo(StoreContext context)
        {
            _context = context;
        }        

        public async Task<IReadOnlyList<Product>> GetProductsAsync()
        {
            return await _context.Product
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product> GetProductsByIdAsync(int id)
        {
            return await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Category>> GetCategoryAsync()
        {
            return await _context.Category.ToListAsync();
        }

        public List<Product> TopSellingProducts()
        {
            //var query = from prod in _context.Product
            //            join ordItem in _context.OrderItem
            //            on prod.Id equals ordItem.ProductId
            //            orderby prod.Quantity
            //            select prod;


            var topSellingProducts = _context.OrderItem
                   .GroupBy(o => o.ProductId)
                   .Select(g => new
                   {
                       ProductId = g.Key,
                       TotalQuantitySold = g.Sum(o => o.Quantity)
                   })
                   .OrderByDescending(p => p.TotalQuantitySold)
                   .Take(10) // You can change this to get top N selling products
                   .ToList();

            var topSellingProductsInfo =  (from ts in topSellingProducts
                                          join p in _context.Product on ts.ProductId equals p.Id
                                          select p)
                                          .ToList();



            return topSellingProductsInfo;   
                        
        }
    }
}
