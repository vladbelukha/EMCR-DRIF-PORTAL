using AutoMapper;
using EMCR.DRR.Controllers;

namespace EMCR.DRR.API.Mappers
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Managers.Intake.DeclarationInfo, DeclarationInfo>()
                ;
        }
    }
}
