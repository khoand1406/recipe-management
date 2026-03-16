using AutoMapper;
using RecipeMgt.Application.DTOs.Response.Recipe;
using RecipeMgt.Application.DTOs.Response.User;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Mapper
{
    public class UserProfile:Profile
    {
        public UserProfile() {
            CreateMap<User, UserBasicResponse>().ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
            CreateMap<User, UserResponseDTO>();
            CreateMap<User, UserResponseMgtDTO>().ForMember(dest=> dest.TotalRecipes, opt=> opt.MapFrom(src=> src.Recipes.Count))
                .ForMember(dest=> dest.TotalFollowers, opt=> opt.MapFrom(src=> src.Followers.Count));

            CreateMap<User, UserDetailResponse>().ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Ratings.Count))
                                                .ForMember(dest => dest.TotalFollowers, opt => opt.MapFrom(src => src.Followers.Count))
                                                .ForMember(dest => dest.TotalFollowing, opt => opt.MapFrom(src => src.FollowingUsers.Count))
                                                .ForMember(dest => dest.TotalRecipes, opt => opt.MapFrom(src => src.Recipes.Count));
                                                

        }
    }
}
