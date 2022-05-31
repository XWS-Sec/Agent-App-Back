using AgentApi.Dtos;
using AgentApi.Model;
using AutoMapper;

namespace AgentApi.MappingProfiles
{
    public class GeneralMappingProfile : Profile
    {
        public GeneralMappingProfile()
        {
            CreateMap<RegisterUserDto, User>().ReverseMap();
            CreateMap<RegisterCompanyDto, Company>();
            CreateMap<NewJobOfferDto, JobOffer>();
            CreateMap<Company, SearchCompanyDto>();
        }
    }
}