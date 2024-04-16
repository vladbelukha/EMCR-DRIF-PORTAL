using AutoMapper;
using EMCR.DRR.Controllers;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.Resources.Applications
{
    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            CreateMap<EOIApplication, drr_application>(MemberList.None)
                .ForMember(dest => dest.drr_primaryapplicanttype, opt => opt.MapFrom(src => (int?)Enum.Parse<ApplicantTypeOptionSet>(src.ApplicantType.ToString())))
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.ApplicantName))
                .ForMember(dest => dest.drr_PrimaryApplicant, opt => opt.MapFrom(src => new account { name = src.ApplicantName }))
                .ForMember(dest => dest.drr_SubmitterContactInformation, opt => opt.MapFrom(src => src.Submitter))
                .ForMember(dest => dest.drr_application_contact_Application, opt => opt.MapFrom(src => src.ProjectContacts))
                .ForMember(dest => dest.drr_projecttitle, opt => opt.MapFrom(src => src.ProjectTitle))
                .ForMember(dest => dest.drr_projecttype, opt => opt.MapFrom(src => (int?)Enum.Parse<ProjectTypeOptionSet>(src.ProjectType.ToString())))
                .ForMember(dest => dest.drr_hazards, opt => opt.MapFrom(src => string.Join(",", src.RelatedHazards.Select(h => (int?)Enum.Parse<HazardsOptionSet>(h.ToString())))))
                .ForMember(dest => dest.drr_reasonswhyotherselectedforhazards, opt => opt.MapFrom(src => src.OtherHazardsDescription))
                .ForMember(dest => dest.drr_anticipatedprojectstartdate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.drr_anticipatedprojectenddate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.drr_driffundingrequest, opt => opt.MapFrom(src => src.FundingRequest))
                .ForMember(dest => dest.drr_application_fundingsource_Application, opt => opt.MapFrom(src => src.OtherFunding))
                .ForMember(dest => dest.drr_unfundedamount, opt => opt.MapFrom(src => src.UnfundedAmount))
                .ForMember(dest => dest.drr_reasonstosecurefunding, opt => opt.MapFrom(src => src.ReasonsToSecureFunding))
                .ForMember(dest => dest.drr_totalfundingsources, opt => opt.MapFrom(src => src.TotalFunding))
                .ForMember(dest => dest.drr_ownershipdeclaration, opt => opt.MapFrom(src => src.OwnershipDeclaration ? DRRTwoOptions.Yes : DRRTwoOptions.No))
                .ForMember(dest => dest.drr_locationdescription, opt => opt.MapFrom(src => src.LocationInformation.Description))
                .ForMember(dest => dest.drr_sizeofprojectarea, opt => opt.MapFrom(src => src.LocationInformation.Area))
                .ForMember(dest => dest.drr_landuseorownership, opt => opt.MapFrom(src => src.LocationInformation.Ownership))
                .ForMember(dest => dest.drr_backgroundforfundingrequest, opt => opt.MapFrom(src => src.BackgroundDescription))
                .ForMember(dest => dest.drr_rationaleforfundingrequest, opt => opt.MapFrom(src => src.RationaleForFunding))
                .ForMember(dest => dest.drr_proposedsolution, opt => opt.MapFrom(src => src.ProposedSolution))
                .ForMember(dest => dest.drr_rationalforproposedsolution, opt => opt.MapFrom(src => src.RationaleForSolution))
                .ForMember(dest => dest.drr_engagementwithfirstnationsorindigenousorg, opt => opt.MapFrom(src => src.EngagementProposal))
                .ForMember(dest => dest.drr_climateadaptation, opt => opt.MapFrom(src => src.ClimateAdaptation))
                .ForMember(dest => dest.drr_otherrelevantinformation, opt => opt.MapFrom(src => src.OtherInformation))
            ;

            CreateMap<FundingInformation, drr_fundingsource>()
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.drr_typeoffunding, opt => opt.MapFrom(src => (int?)Enum.Parse<FundingTypeOptionSet>(src.Type.ToString())))
                .ForMember(dest => dest.drr_amount, opt => opt.MapFrom(src => src.Amount))
            ;

            CreateMap<ContactDetails, contact>()
                .ForMember(dest => dest.firstname, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.lastname, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.jobtitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.department, opt => opt.MapFrom(src => src.Department))
                .ForMember(dest => dest.drr_phonenumber, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.emailaddress1, opt => opt.MapFrom(src => src.Email))
            ;
        }
    }
}
