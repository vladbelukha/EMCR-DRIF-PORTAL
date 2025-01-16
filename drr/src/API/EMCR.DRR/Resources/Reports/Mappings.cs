using AutoMapper;
using EMCR.DRR.API.Resources.Projects;
using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Cases
{
    public class ReportMapperProfile : Profile
    {
        public ReportMapperProfile()
        {
#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            CreateMap<InterimReportDetails, drr_projectreport>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.drr_progressdescription))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.drr_reportdate.HasValue ? src.drr_reportdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<InterimReportStatus>(((ProjectReportStatusOptionSet)src.statuscode).ToString())))
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.drr_projecttype.HasValue ? (int?)Enum.Parse<InterimProjectType>(((FundingStreamOptionSet)src.drr_projecttype).ToString()) : null))
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.drr_periodtype.HasValue ? (int?)Enum.Parse<PeriodType>(((PeriodTypeOptionSet)src.drr_periodtype).ToString()) : null))
                .ForMember(dest => dest.ProjectClaim, opt => opt.MapFrom(src => src.drr_ClaimReport))
                .ForMember(dest => dest.ProgressReport, opt => opt.MapFrom(src => src.drr_ProgressReport))
                .ForMember(dest => dest.Forecast, opt => opt.MapFrom(src => src.drr_BudgetForecast))
            ;

            CreateMap<ClaimDetails, drr_projectclaim>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ClaimAmount, opt => opt.MapFrom(src => src.drr_claimamount))
                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src => src.drr_dateapproved.HasValue ? src.drr_dateapproved.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ClaimType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ClaimStatus>(((ProjectClaimStatusOptionSet)src.statuscode).ToString())))
            ;

            CreateMap<ProgressReportDetails, drr_projectprogress>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.DateApproved, opt => opt.MapFrom(src => src.drr_dateapproved.HasValue ? src.drr_dateapproved.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.DateSubmitted, opt => opt.MapFrom(src => src.drr_datesubmitted.HasValue ? src.drr_datesubmitted.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.drr_duedate.HasValue ? src.drr_duedate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.WorkplanActivities, opt => opt.MapFrom(src => src.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport))
                .ForMember(dest => dest.ReportType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ProgressReportStatus>(((ProjectProgressReportStatusOptionSet)src.statuscode).ToString())))
            ;

            CreateMap<ForecastDetails, drr_projectbudgetforecast>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ForecastAmount, opt => opt.MapFrom(src => src.drr_amountspent))
                .ForMember(dest => dest.ForecastDate, opt => opt.MapFrom(src => src.drr_reportasatdate.HasValue ? src.drr_reportasatdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ForecastType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
            ;

            CreateMap<WorkplanActivityDetails, drr_projectworkplanactivity>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.Ignore())
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => src.drr_progressstatus.HasValue ? (int?)Enum.Parse<WorkplanProgress>(((WorkplanProgressOptionSet)src.drr_progressstatus).ToString()) : null))
            ;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
