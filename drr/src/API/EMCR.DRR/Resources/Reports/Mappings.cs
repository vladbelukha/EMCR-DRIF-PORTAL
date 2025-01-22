using AutoMapper;
using EMCR.DRR.API.Resources.Projects;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Reports
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
                .ForPath(dest => dest.Workplan.WorkplanActivities, opt => opt.MapFrom(src => src.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport.Where(c => c.statecode == (int)EntityState.Active)))
                .ForPath(dest => dest.Workplan.ProjectCompletionPercentage, opt => opt.MapFrom(src => src.drr_percentageofprojectcompleteasofreportdate))
                .ForPath(dest => dest.Workplan.CommunityMedia, opt => opt.Ignore())
                .ForPath(dest => dest.Workplan.CommunityMediaDate, opt => opt.Ignore())
                .ForPath(dest => dest.Workplan.CommunityMediaComment, opt => opt.MapFrom(src => src.drr_commentscommunitymediaannouncements))
                .ForPath(dest => dest.Workplan.ProvincialMedia, opt => opt.MapFrom(src => Enum.Parse<ProvincialMedia>(((ProvincialMediaOptionSet)src.drr_provincialmediaannouncements).ToString())))
                .ForPath(dest => dest.Workplan.ProvincialMediaDate, opt => opt.Ignore())
                .ForPath(dest => dest.Workplan.ProvincialMediaComment, opt => opt.MapFrom(src => src.drr_commentsprovincialmediaannouncements))
                .ForPath(dest => dest.Workplan.WorksCompleted, opt => opt.Ignore())
                .ForPath(dest => dest.Workplan.OutstandingIssues, opt => opt.MapFrom(src => src.drr_outstandingissues))
                .ForPath(dest => dest.Workplan.FundingSourcesChanged, opt => opt.MapFrom(src => src.drr_changestofundingsources.HasValue ? src.drr_changestofundingsources.Value == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForPath(dest => dest.Workplan.FundingSourcesChangedComment, opt => opt.MapFrom(src => src.drr_commentschangestofundingsources))
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
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.statuscode.HasValue ? (int?)Enum.Parse<WorkplanStatus>(((WorkplanProgressOptionSet)src.statuscode).ToString()) : null))
                .ForMember(dest => dest.PlannedStartDate, opt => opt.MapFrom(src => src.drr_plannedstartdate.HasValue ? src.drr_plannedstartdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.PlannedCompletionDate, opt => opt.MapFrom(src => src.drr_plannedcompletiondate.HasValue ? src.drr_plannedcompletiondate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ActualStartDate, opt => opt.MapFrom(src => src.drr_actualstartdate.HasValue ? src.drr_actualstartdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ActualCompletionDate, opt => opt.MapFrom(src => src.drr_actualcompletiondate.HasValue ? src.drr_actualcompletiondate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.drr_ActivityType))
                .ForMember(dest => dest.Comment, opt => opt.Ignore())
            ;

            CreateMap<ActivityType, drr_projectactivity>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.Ignore())
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.PreCreatedActivity, opt => opt.MapFrom(src => src.drr_precreatedactivity.HasValue ? src.drr_precreatedactivity == (int)DRRTwoOptions.Yes : false))
            ;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
