using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Mapper
{
    public class DishProfile : Profile
    {
        public DishProfile()
        {
            CreateMap<CreateDishRequest, Dish>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<UpdateDishRequest, Dish>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<Dish, DishResponse>().ForMember(dest => dest.BookmarkCount,
                opt => opt.MapFrom(d => d.Statistic != null ? d.Statistic.BookmarkCount : 0))
                .ForMember(dest => dest.ViewCount,
                opt => opt.MapFrom(d => d.Statistic != null ? d.Statistic.ViewCount : 0))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(d =>d.Images != null
                ? d.Images.Select(i => i.ImageUrl)
                : new List<string>()));

            CreateMap<Dish, DishDetailResponse>()
             .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(d => d.Images != null ? d.Images.Select(i=> i.ImageUrl): new List<string>()));

            
        }
    }
}
