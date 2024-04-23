using AutoMapper;
using EMCR.DRR.Controllers;

namespace EMCR.DRR.Managers.Intake
{
    public class IntakeMapperProfile : Profile
    {
        public IntakeMapperProfile()
        {
            CreateMap<DrifEoiApplication, Application>();
            CreateMap<Controllers.FundingInformation, FundingInformation>();
            CreateMap<Controllers.ContactDetails, ContactDetails>();
            CreateMap<string, PartneringProponent>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src));
        }
    }
}
