using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Dishes;
using RecipeMgt.Application.DTOs.Response.Dishes;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Mapper
{
    public class DishProfile:Profile
    {
        public DishProfile() {
            CreateMap<CreateDishRequest, Dish>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<UpdateDishRequest, Dish>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<Dish, DishResponse>();

            CreateMap<Dish, DishDetailResponse>();
           
        }
    }
}
