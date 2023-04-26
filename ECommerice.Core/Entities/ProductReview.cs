using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Core.Entities
{
    public class ProductReview: BaseEntity
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        //Relations
        public Product Product { get; set; }
    }
}
