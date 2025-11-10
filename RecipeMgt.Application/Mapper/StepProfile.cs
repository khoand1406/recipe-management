using AutoMapper;
using RecipeMgt.Application.DTOs.Request.Steps;
using RecipeMgt.Application.DTOs.Response.Steps;
using RecipeMgt.Domain.Entities;

namespace RecipeMgt.Application.Mapper
{
    public class StepProfile : Profile
    {
        public StepProfile()
        {
            CreateMap<Step, StepResponse>();
            CreateMap<CreateStepRequest, Step>();
            CreateMap<UpdateStepRequest, Step>();
        }
    }
}
