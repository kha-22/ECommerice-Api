﻿using AutoMapper;
using ECommerice.Api.DTOs;
using ECommerice.Core.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerice.Api.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDTO, string>
    {
        public readonly IConfiguration _config;
        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Product source, ProductToReturnDTO destination, string destMember, ResolutionContext context)
        {
            //if (!string.IsNullOrEmpty(source.PictureUrl))
            //{
            //    return _config["ApiUrl"] + source.PictureUrl;
            //}
            return null;
        }
    }
}
