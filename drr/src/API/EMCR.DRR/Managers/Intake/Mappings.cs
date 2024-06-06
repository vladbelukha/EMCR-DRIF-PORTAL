using AutoMapper;
using EMCR.DRR.Controllers;

namespace EMCR.DRR.Managers.Intake
{
    public class IntakeMapperProfile : Profile
    {
        public IntakeMapperProfile()
        {
            CreateMap<DrifEoiApplication, Application>()
                .ForMember(dest => dest.AdditionalContact1, opt => opt.MapFrom(src => src.AdditionalContacts.FirstOrDefault()))
                .ForMember(dest => dest.AdditionalContact2, opt => opt.MapFrom(src => src.AdditionalContacts.ElementAtOrDefault(1)))
                ;
            CreateMap<Controllers.FundingInformation, FundingInformation>();
            CreateMap<Controllers.ContactDetails, ContactDetails>();
            CreateMap<string, PartneringProponent>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src));
            CreateMap<string, CriticalInfrastructure>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src));

            CreateMap<Resources.Applications.DeclarationInfo, DeclarationInfo>()
                ;
        }
    }
}
