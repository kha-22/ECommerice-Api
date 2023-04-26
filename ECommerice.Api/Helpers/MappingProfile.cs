﻿using AutoMapper;
using ECommerice.Api.DTOs;
using ECommerice.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerice.Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(pb => pb.Category, p => p.MapFrom(s => s.Category.Name))
                .ForMember(d => d.Url,  o => o.MapFrom(p => p.ProductImages.Where(p => p.IsMain == true).FirstOrDefault().Url))
                .ForMember(d => d.ProductImages,  o => o.MapFrom(p => p.ProductImages));

            CreateMap<Address, AddressDTO>().ReverseMap();
            //CreateMap<CustomerBasketDTO, CustomerBasket>();
            CreateMap<BasketItemsDTO, BasketItem>(); 
            CreateMap<AddressDTO, Core.Entities.OrderAggregate.Address>();

            CreateMap<AppUser, UsersDTO>().ReverseMap();
            CreateMap<ProductImage, ProductImageDto>().ReverseMap(); 
            CreateMap<ProductReview, ProductReviewDto>().ReverseMap(); 

        }
    }
}
