using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Comments;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Mapper
{
    public class CommentProfile: Profile
    {
        public CommentProfile() {
            CreateMap<Comment, CreateCommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();
        }
    }
}
