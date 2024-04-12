using AutoMapper;
using EMCR.DRR.Controllers;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.Resources.Applications
{
    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            CreateMap<EOIApplication, Drr_drifapplication>(MemberList.None)
                .ForMember(dest => dest.Drr_applicanttype, opts => opts.MapFrom(src => (int?)Enum.Parse<ApplicantTypeOptionSet>(src.ApplicantType.ToString())))
                .ForMember(dest => dest.Drr_name, opt => opt.MapFrom(src => src.ApplicantName))
                //Submitter
                //ProjectContacts
                .ForMember(dest => dest.Drr_projecttitle, opt => opt.MapFrom(src => src.ProjectTitle))
                //Project Type
                //Related Hazards
                //Start Date
                //End Date
                .ForMember(dest => dest.Drr_driffundingrequest, opt => opt.MapFrom(src => src.FundingRequest))
                //Other Funding
                //Unfunded
                .ForMember(dest => dest.Drr_totalprojectcost, opt => opt.MapFrom(src => src.TotalFunding))
                .ForMember(dest => dest.Drr_ownershipdeclaration, opt => opt.MapFrom(src => src.OwnershipDeclaration ? DRRTwoOptions.Yes : DRRTwoOptions.No))
                //LocationDescription
                //Coordinates
                //Area
                //Units
                //Ownership
                .ForMember(dest => dest.Drr_backgroundforfundingrequest, opt => opt.MapFrom(src => src.BackgroundDescription))
                .ForMember(dest => dest.Drr_rationaleforfundingrequest, opt => opt.MapFrom(src => src.RationaleForFunding))
                .ForMember(dest => dest.Drr_proposedsolution, opt => opt.MapFrom(src => src.ProposedSolution))
                .ForMember(dest => dest.Drr_rationaleforproposedsolution, opt => opt.MapFrom(src => src.RationaleForSolution))
                .ForMember(dest => dest.Drr_engagementwithfirstnations, opt => opt.MapFrom(src => src.EngagementProposal))
            //ClimateAdaptation
            //OtherInformation
            //IdentityConfirmation
            //FOIPPAConfirmation
            //CFOConfirmation
            ;

            CreateMap<ContactDetails, Contact>()
                .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.FirstName))
            ;
        }
    }
}
