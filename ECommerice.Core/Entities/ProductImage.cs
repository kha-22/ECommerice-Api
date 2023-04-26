﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Core.Entities
{
    public class ProductImage: BaseEntity
    {
        public string Url { get; set; }
        public bool IsMain { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }
    }
}
