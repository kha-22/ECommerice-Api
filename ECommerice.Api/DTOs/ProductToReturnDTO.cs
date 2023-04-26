using ECommerice.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerice.Api.DTOs
{
    public class ProductToReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int  Quantity{ get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Url { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public List<ProductImageDto> ProductImages { get; set; }
        public List<ProductReviewDto> ProductReviews { get; set; }
        
    }
}
