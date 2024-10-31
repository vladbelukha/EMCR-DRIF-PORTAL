using AutoMapper;
using EMCR.DRR.API.Resources.Documents;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Cases
{
    public class DocumentMapperProfile : Profile
    {
        public DocumentMapperProfile()
        {
            CreateMap<Document, bcgov_documenturl>(MemberList.None)
                .ForMember(dest => dest.bcgov_filename, opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.bcgov_filename))
                .ForMember(dest => dest.DocumentType, opt => opt.Ignore())
                ;
        }
    }
}
