using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Follows;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Mapper
{
    public class FollowProfile: Profile
    {
        public FollowProfile() {
            CreateMap<Following, CreateFollowDTO>();
            CreateMap<CreateFollowDTO, Following>();
        }
    }
}
