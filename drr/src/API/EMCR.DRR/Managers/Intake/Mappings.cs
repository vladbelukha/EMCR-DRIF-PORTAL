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
            CreateMap<Controllers.LocationInformation, LocationInformation>();
            CreateMap<Controllers.ContactDetails, ContactDetails>();
        }
    }
}
