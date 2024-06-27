using AutoMapper;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Accounts
{
    public class AccountMapperProfile : Profile
    {
        public AccountMapperProfile()
        {
            CreateMap<Account, account>(MemberList.None)
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.drr_bceidguid, opt => opt.MapFrom(src => src.BCeIDBusinessId))
                ;
        }
    }
}
