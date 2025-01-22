using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.API.Resources.Projects;
using EMCR.DRR.Controllers;
using EMCR.Utilities.Extensions;

namespace EMCR.DRR.Managers.Intake
{
    public class IntakeMapperProfile : Profile
    {
        public IntakeMapperProfile()
        {
            CreateMap<EoiApplication, Application>(MemberList.None)
                .ForMember(dest => dest.AdditionalContact1, opt => opt.MapFrom(src => src.AdditionalContacts.FirstOrDefault()))
                .ForMember(dest => dest.AdditionalContact2, opt => opt.MapFrom(src => src.AdditionalContacts.ElementAtOrDefault(1)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IntakeStatusMapper(src.Status)))
                .ForMember(dest => dest.ApplicationTypeName, opt => opt.MapFrom(src => "EOI"))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => "DRIF"))
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.Stream))
                .ReverseMap()
                .ForMember(dest => dest.AdditionalContacts, opt => opt.MapFrom(src => DRRAdditionalContactMapper(src.AdditionalContact1, src.AdditionalContact2)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DRRApplicationStatusMapper(src.Status)))
                .ForMember(dest => dest.PartneringProponents, opt => opt.MapFrom(src => src.PartneringProponents.Select(p => p.Name)))
                .ForMember(dest => dest.Stream, opt => opt.MapFrom(src => src.ProjectType))
                ;

            CreateMap<DraftEoiApplication, Application>(MemberList.None)
                .ForMember(dest => dest.AdditionalContact1, opt => opt.MapFrom(src => src.AdditionalContacts.FirstOrDefault()))
                .ForMember(dest => dest.AdditionalContact2, opt => opt.MapFrom(src => src.AdditionalContacts.ElementAtOrDefault(1)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IntakeStatusMapper(src.Status)))
                .ForMember(dest => dest.ApplicationTypeName, opt => opt.MapFrom(src => "EOI"))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => "DRIF"))
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.Stream))
                .ReverseMap()
                .ForMember(dest => dest.AdditionalContacts, opt => opt.MapFrom(src => DRRAdditionalContactMapper(src.AdditionalContact1, src.AdditionalContact2)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DRRApplicationStatusMapper(src.Status)))
                .ForMember(dest => dest.PartneringProponents, opt => opt.MapFrom(src => src.PartneringProponents.Select(p => p.Name)))
                .ForMember(dest => dest.Stream, opt => opt.MapFrom(src => src.ProjectType))
                ;

            CreateMap<FpApplication, Application>(MemberList.None)
                .ForMember(dest => dest.AdditionalContact1, opt => opt.MapFrom(src => src.AdditionalContacts.FirstOrDefault()))
                .ForMember(dest => dest.AdditionalContact2, opt => opt.MapFrom(src => src.AdditionalContacts.ElementAtOrDefault(1)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IntakeStatusMapper(src.Status)))
                .ForMember(dest => dest.ApplicationTypeName, opt => opt.MapFrom(src => "Full Proposal"))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => "DRIF"))
                .ForMember(dest => dest.EstimatedTotal, opt => opt.MapFrom(src => src.TotalProjectCost))
                .AfterMap((_, dest) =>
                {
                    int i = 1;
                    if (dest.ProposedActivities != null)
                    {
                        foreach (var activity in dest.ProposedActivities)
                        {
                            activity.ActivityNumber = i++;
                        }
                    }
                })
                .ReverseMap()
                .ForMember(dest => dest.AdditionalContacts, opt => opt.MapFrom(src => DRRAdditionalContactMapper(src.AdditionalContact1, src.AdditionalContact2)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DRRApplicationStatusMapper(src.Status)))
                .ForMember(dest => dest.PartneringProponents, opt => opt.MapFrom(src => src.PartneringProponents.Select(p => p.Name)))
                .ForMember(dest => dest.Professionals, opt => opt.MapFrom(src => src.Professionals.Select(p => p.Name)))
                .ForMember(dest => dest.FoundationalOrPreviousWorks, opt => opt.MapFrom(src => src.FoundationalOrPreviousWorks.Select(p => p.Name)))
                .ForMember(dest => dest.AffectedParties, opt => opt.MapFrom(src => src.AffectedParties.Select(p => p.Name)))
                .ForMember(dest => dest.CostReductions, opt => opt.MapFrom(src => src.CostReductions.Select(p => p.Name)))
                .ForMember(dest => dest.CoBenefits, opt => opt.MapFrom(src => src.CoBenefits.Select(p => p.Name)))
                .ForMember(dest => dest.IncreasedResiliency, opt => opt.MapFrom(src => src.IncreasedResiliency.Select(p => p.Name)))
                .ForMember(dest => dest.ComplexityRisks, opt => opt.MapFrom(src => src.ComplexityRisks.Select(p => p.Name)))
                .ForMember(dest => dest.ReadinessRisks, opt => opt.MapFrom(src => src.ReadinessRisks.Select(p => p.Name)))
                .ForMember(dest => dest.SensitivityRisks, opt => opt.MapFrom(src => src.SensitivityRisks.Select(p => p.Name)))
                .ForMember(dest => dest.CapacityRisks, opt => opt.MapFrom(src => src.CapacityRisks.Select(p => p.Name)))
                .ForMember(dest => dest.ClimateAssessmentTools, opt => opt.MapFrom(src => src.ClimateAssessmentTools.Select(p => p.Name)))
                .ForMember(dest => dest.CostConsiderations, opt => opt.MapFrom(src => src.CostConsiderations.Select(p => p.Name)))
                .ForMember(dest => dest.TotalProjectCost, opt => opt.MapFrom(src => src.EstimatedTotal))
                //.ForMember(dest => dest.EligibleFundingRequest, opt => opt.MapFrom(src => src.EligibleAmountForFP))
                .ForMember(dest => dest.Permits, opt => opt.MapFrom(src => src.Permits.Select(p => p.Name)))
                ;

            CreateMap<DraftFpApplication, Application>(MemberList.None)
                .ForMember(dest => dest.AdditionalContact1, opt => opt.MapFrom(src => src.AdditionalContacts.FirstOrDefault()))
                .ForMember(dest => dest.AdditionalContact2, opt => opt.MapFrom(src => src.AdditionalContacts.ElementAtOrDefault(1)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => IntakeStatusMapper(src.Status)))
                .ForMember(dest => dest.ApplicationTypeName, opt => opt.MapFrom(src => "Full Proposal"))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => "DRIF"))
                .ForMember(dest => dest.EstimatedTotal, opt => opt.MapFrom(src => src.TotalProjectCost))
                .AfterMap((_, dest) =>
                {
                    int i = 1;
                    if (dest.ProposedActivities != null)
                    {
                        foreach (var activity in dest.ProposedActivities)
                        {
                            activity.ActivityNumber = i++;
                        }
                    }
                })
                .ReverseMap()
                .ForMember(dest => dest.AdditionalContacts, opt => opt.MapFrom(src => DRRAdditionalContactMapper(src.AdditionalContact1, src.AdditionalContact2)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DRRApplicationStatusMapper(src.Status)))
                .ForMember(dest => dest.PartneringProponents, opt => opt.MapFrom(src => src.PartneringProponents.Select(p => p.Name)))
                .ForMember(dest => dest.Professionals, opt => opt.MapFrom(src => src.Professionals.Select(p => p.Name)))
                .ForMember(dest => dest.FoundationalOrPreviousWorks, opt => opt.MapFrom(src => src.FoundationalOrPreviousWorks.Select(p => p.Name)))
                .ForMember(dest => dest.AffectedParties, opt => opt.MapFrom(src => src.AffectedParties.Select(p => p.Name)))
                .ForMember(dest => dest.CostReductions, opt => opt.MapFrom(src => src.CostReductions.Select(p => p.Name)))
                .ForMember(dest => dest.CoBenefits, opt => opt.MapFrom(src => src.CoBenefits.Select(p => p.Name)))
                .ForMember(dest => dest.IncreasedResiliency, opt => opt.MapFrom(src => src.IncreasedResiliency.Select(p => p.Name)))
                .ForMember(dest => dest.ComplexityRisks, opt => opt.MapFrom(src => src.ComplexityRisks.Select(p => p.Name)))
                .ForMember(dest => dest.ReadinessRisks, opt => opt.MapFrom(src => src.ReadinessRisks.Select(p => p.Name)))
                .ForMember(dest => dest.SensitivityRisks, opt => opt.MapFrom(src => src.SensitivityRisks.Select(p => p.Name)))
                .ForMember(dest => dest.CapacityRisks, opt => opt.MapFrom(src => src.CapacityRisks.Select(p => p.Name)))
                .ForMember(dest => dest.ClimateAssessmentTools, opt => opt.MapFrom(src => src.ClimateAssessmentTools.Select(p => p.Name)))
                .ForMember(dest => dest.CostConsiderations, opt => opt.MapFrom(src => src.CostConsiderations.Select(p => p.Name)))
                .ForMember(dest => dest.TotalProjectCost, opt => opt.MapFrom(src => src.EstimatedTotal))
                //.ForMember(dest => dest.EligibleFundingRequest, opt => opt.MapFrom(src => src.EligibleAmountForFP))
                .ForMember(dest => dest.Permits, opt => opt.MapFrom(src => src.Permits.Select(p => p.Name)))
                ;

            CreateMap<DraftDrrProject, Project>(MemberList.None)
                .ReverseMap()
                ;

            CreateMap<Controllers.PaymentCondition, PaymentCondition>()
                .ReverseMap()
                ;

            CreateMap<Controllers.ProjectClaim, ProjectClaim>()
                .ReverseMap()
                ;

            CreateMap<Controllers.ProgressReport, ProgressReport>()
                .ReverseMap()
                ;

            CreateMap<Controllers.Forecast, Forecast>()
                .ReverseMap()
                ;

            CreateMap<Controllers.InterimReport, InterimReport>()
                .ReverseMap()
                ;

            CreateMap<Controllers.WorkplanActivity, WorkplanActivity>()
                .ReverseMap()
                ;

            CreateMap<Controllers.ProjectClaim, ClaimDetails>()
                .ReverseMap()
                ;

            CreateMap<Controllers.ProgressReport, ProgressReportDetails>()
                .ReverseMap()
                ;

            CreateMap<Controllers.Forecast, ForecastDetails>()
                .ReverseMap()
                ;

            CreateMap<Controllers.InterimReport, InterimReportDetails>()
                .ReverseMap()
                ;

            CreateMap<Controllers.WorkPlan, WorkPlanDetails>()
                .ReverseMap()
                ;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
            CreateMap<Controllers.WorkplanActivity, WorkplanActivityDetails>()
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => new ActivityType { Name = src.Activity.ToString(), PreCreatedActivity = src.PreCreatedActivity }))
                .ReverseMap()
                //.ForMember(dest => dest.Activity, opt => opt.MapFrom(src => IEnumEx.GetValueFromDescription<DocumentType>(src.bcgov_DocumentType != null ? src.bcgov_DocumentType.bcgov_name : DocumentType.OtherSupportingDocument.ToDescriptionString())))
                .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => IEnumEx.GetValueFromDescription<Controllers.ActivityType>(src.ActivityType.Name)))
                .ForMember(dest => dest.PreCreatedActivity, opt => opt.MapFrom(src => src.ActivityType.PreCreatedActivity))
                ;
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            CreateMap<Controllers.ProjectEvent, ProjectEvent>()
                .ReverseMap()
                ;

            CreateMap<Controllers.FundingInformation, FundingInformation>()
                .ReverseMap()
                ;

            CreateMap<Controllers.YearOverYearFunding, YearOverYearFunding>()
                .ReverseMap()
                ;

            CreateMap<Controllers.ScreenerQuestions, ScreenerQuestions>()
                .ReverseMap()
                ;

            CreateMap<Controllers.ContactDetails, ContactDetails>()
                .ForMember(dest => dest.BCeId, opt => opt.Ignore())
                .ReverseMap()
                ;

            CreateMap<Controllers.StandardInfo, StandardInfo>()
                .ReverseMap()
                .ForMember(dest => dest.Standards, opt => opt.MapFrom(src => src.Standards.Select(p => p.Name)))
                ;

            CreateMap<string, PartneringProponent>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
                ;

            CreateMap<Controllers.InfrastructureImpacted, CriticalInfrastructure>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Infrastructure))
                .ReverseMap()
                .ForMember(dest => dest.Infrastructure, opt => opt.MapFrom(src => src.Name))
                ;

            CreateMap<string, ProfessionalInfo>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, ProvincialStandard>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, CostReduction>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, CoBenefit>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, IncreasedResiliency>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, FoundationalOrPreviousWork>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, AffectedParty>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, ComplexityRisk>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, ReadinessRisk>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, SensitivityRisk>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, CapacityRisk>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, TransferRisks>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, ClimateAssessmentToolsInfo>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, CostConsideration>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<string, Permit>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src))
               ;

            CreateMap<Controllers.ProposedActivity, ProposedActivity>()
                .ForMember(dest => dest.ActivityNumber, opt => opt.Ignore())
                .ReverseMap()
                ;

            CreateMap<Controllers.CostEstimate, CostEstimate>()
                .ReverseMap()
                ;

            CreateMap<Resources.Applications.EntitiesQueryResult, EntitiesQueryResult>()
                .ReverseMap()
                ;

            CreateMap<Attachment, BcGovDocument>()
                .ReverseMap()
                ;
        }

        private IEnumerable<ContactDetails> DRRAdditionalContactMapper(ContactDetails? contact1, ContactDetails? contact2)
        {
            var ret = new List<ContactDetails>();
            if (contact1 != null) ret.Add(contact1);
            if (contact2 != null) ret.Add(contact2);
            return ret;
        }

        private SubmissionPortalStatus DRRApplicationStatusMapper(ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Approved:
                    return SubmissionPortalStatus.Approved;
                case ApplicationStatus.ApprovedInPrinciple:
                    return SubmissionPortalStatus.ApprovedInPrinciple;
                case ApplicationStatus.Closed:
                    return SubmissionPortalStatus.Closed;
                case ApplicationStatus.DraftProponent:
                case ApplicationStatus.DraftStaff:
                    return SubmissionPortalStatus.Draft;
                case ApplicationStatus.Invited:
                    return SubmissionPortalStatus.EligibleInvited;
                case ApplicationStatus.InPool:
                    return SubmissionPortalStatus.EligiblePending;
                case ApplicationStatus.FPSubmitted:
                    return SubmissionPortalStatus.FullProposalSubmitted;
                case ApplicationStatus.Ineligible:
                    return SubmissionPortalStatus.Ineligible;
                case ApplicationStatus.Submitted:
                case ApplicationStatus.InReview:
                    return SubmissionPortalStatus.UnderReview;
                case ApplicationStatus.Withdrawn:
                    return SubmissionPortalStatus.Withdrawn;
                case ApplicationStatus.Deleted:
                    return SubmissionPortalStatus.Deleted;
                default:
                    return SubmissionPortalStatus.Draft;
            }
        }

        private ApplicationStatus IntakeStatusMapper(SubmissionPortalStatus? status)
        {
            switch (status)
            {
                case SubmissionPortalStatus.Draft:
                    return ApplicationStatus.DraftProponent;
                case SubmissionPortalStatus.UnderReview:
                    return ApplicationStatus.Submitted;
                case SubmissionPortalStatus.EligibleInvited:
                    return ApplicationStatus.Invited;
                case SubmissionPortalStatus.EligiblePending:
                    return ApplicationStatus.InPool;
                case SubmissionPortalStatus.Ineligible:
                    return ApplicationStatus.Ineligible;
                case SubmissionPortalStatus.Withdrawn:
                    return ApplicationStatus.Withdrawn;
                default:
                    return ApplicationStatus.DraftProponent;
            }
        }
    }
}
