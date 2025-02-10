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
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.drr_projecttype.HasValue ? (int?)Enum.Parse<InterimProjectType>(((FundingStreamOptionSet)src.drr_projecttype).ToString()) : null))
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.drr_periodtype.HasValue ? (int?)Enum.Parse<PeriodType>(((PeriodTypeOptionSet)src.drr_periodtype).ToString()) : null))
                .ForMember(dest => dest.ProjectClaim, opt => opt.MapFrom(src => src.drr_ClaimReport))
                .ForMember(dest => dest.ProgressReport, opt => opt.MapFrom(src => src.drr_ProgressReport))
                .ForMember(dest => dest.Forecast, opt => opt.MapFrom(src => src.drr_BudgetForecast))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.statuscode.HasValue ? (int?)Enum.Parse<InterimReportStatus>(((ProjectReportStatusOptionSet)src.statuscode).ToString()) : null))
            ;

            CreateMap<ClaimDetails, drr_projectclaim>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ClaimAmount, opt => opt.MapFrom(src => src.drr_claimamount))
                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src => src.drr_dateapproved.HasValue ? src.drr_dateapproved.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ClaimType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.statuscode.HasValue ? (int?)Enum.Parse<ClaimStatus>(((ProjectClaimStatusOptionSet)src.statuscode).ToString()) : null))
            ;

#pragma warning disable CS8604 // Possible null reference argument.
            CreateMap<ProgressReportDetails, drr_projectprogress>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.drr_dateapproved, opt => opt.MapFrom(src => src.DateApproved.HasValue ? src.DateApproved.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_datesubmitted, opt => opt.MapFrom(src => src.DateSubmitted.HasValue ? src.DateSubmitted.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_duedate, opt => opt.MapFrom(src => src.DueDate.HasValue ? src.DueDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport, opt => opt.MapFrom(src => src.Workplan.WorkplanActivities))
                .ForMember(dest => dest.drr_projectprogress1, opt => opt.MapFrom(src => src.Workplan.ProjectProgress.HasValue ? (int?)Enum.Parse<ProjectProgressOptionSet>(src.Workplan.ProjectProgress.Value.ToString()) : null))
                .ForMember(dest => dest.drr_commentsaheadofschedule, opt => opt.MapFrom(src => src.Workplan.AheadOfScheduleComments))
                .ForMember(dest => dest.drr_reasonfordelay, opt => opt.MapFrom(src => src.Workplan.DelayReason.HasValue ? (int?)Enum.Parse<DelayReasonOptionSet>(src.Workplan.DelayReason.Value.ToString()) : null))
                .ForMember(dest => dest.drr_otherreasonfordelay, opt => opt.MapFrom(src => src.Workplan.OtherDelayReason))
                .ForMember(dest => dest.drr_mitigatingstepstakentoremoveorreducedelay, opt => opt.MapFrom(src => src.Workplan.BehindScheduleMitigatingComments))
                .ForMember(dest => dest.drr_percentageofprojectcompleteasofreportdate, opt => opt.MapFrom(src => src.Workplan.ProjectCompletionPercentage))
                .ForMember(dest => dest.drr_percentconstructioncompleteatreportdate, opt => opt.MapFrom(src => src.Workplan.ConstructionCompletionPercentage))
                .ForMember(dest => dest.drr_temporaryprovincialfundingsignage, opt => opt.MapFrom(src => src.Workplan.SignageRequired.HasValue ? src.Workplan.SignageRequired.Value ? (int?)DRRTwoOptions.Yes : (int?)DRRTwoOptions.No : null))
                .ForMember(dest => dest.drr_whyissignagenotrequired, opt => opt.MapFrom(src => src.Workplan.SignageNotRequiredComments))
                .ForMember(dest => dest.drr_drr_projectprogress_drr_temporaryprovincialfundingsignage_ProjectProgress, opt => opt.MapFrom(src => src.Workplan.FundingSignage))
                .ForMember(dest => dest.drr_mediaannouncements, opt => opt.MapFrom(src => src.Workplan.MediaAnnouncement.HasValue ? src.Workplan.MediaAnnouncement.Value ? (int?)DRRTwoOptions.Yes : (int?)DRRTwoOptions.No : null))
                .ForMember(dest => dest.drr_dateofannouncement, opt => opt.MapFrom(src => src.Workplan.MediaAnnouncementDate.HasValue ? src.Workplan.MediaAnnouncementDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_describeannouncement, opt => opt.MapFrom(src => src.Workplan.MediaAnnouncementComment))
                .ForMember(dest => dest.drr_outstandingissuesyesno, opt => opt.MapFrom(src => src.Workplan.OutstandingIssues.HasValue ? src.Workplan.OutstandingIssues.Value ? (int?)DRRTwoOptions.Yes : (int?)DRRTwoOptions.No : null))
                .ForMember(dest => dest.drr_outstandingissues, opt => opt.MapFrom(src => src.Workplan.OutstandingIssuesComments))
                .ForMember(dest => dest.drr_changestofundingsources, opt => opt.MapFrom(src => src.Workplan.FundingSourcesChanged.HasValue ? src.Workplan.FundingSourcesChanged.Value ? (int?)DRRTwoOptions.Yes : (int?)DRRTwoOptions.No : null))
                .ForMember(dest => dest.drr_commentschangestofundingsources, opt => opt.MapFrom(src => src.Workplan.FundingSourcesChangedComment))
                .ForMember(dest => dest.drr_drr_projectprogress_drr_projectevent_ProjectProgress, opt => opt.MapFrom(src => src.EventInformation.PastEvents))
                .ForMember(dest => dest.bcgov_drr_projectprogress_bcgov_documenturl_ProgressReport, opt => opt.MapFrom(src => src.Attachments))
                //.ForMember(dest => dest.statuscode, opt => opt.MapFrom(src => (int?)Enum.Parse<ProjectProgressReportStatusOptionSet>(src.Status.ToString())))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.CrmId, opt => opt.MapFrom(src => src.drr_projectprogressid))
                .ForMember(dest => dest.DateApproved, opt => opt.MapFrom(src => src.drr_dateapproved.HasValue ? src.drr_dateapproved.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.DateSubmitted, opt => opt.MapFrom(src => src.drr_datesubmitted.HasValue ? src.drr_datesubmitted.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.drr_duedate.HasValue ? src.drr_duedate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.drr_Project != null && src.drr_Project.drr_projecttype.HasValue ? (int?)Enum.Parse<InterimProjectType>(((FundingStreamOptionSet)src.drr_Project.drr_projecttype).ToString()) : null))
                .ForPath(dest => dest.Workplan.WorkplanActivities, opt => opt.MapFrom(src => src.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport.Where(c => c.statecode == (int)EntityState.Active)))
                .ForPath(dest => dest.Workplan.ProjectProgress, opt => opt.MapFrom(src => src.drr_projectprogress1.HasValue ? (int?)Enum.Parse<ProjectProgress>(((ProjectProgressOptionSet)src.drr_projectprogress1).ToString()) : null))
                .ForPath(dest => dest.Workplan.AheadOfScheduleComments, opt => opt.MapFrom(src => src.drr_commentsaheadofschedule))
                .ForPath(dest => dest.Workplan.DelayReason, opt => opt.MapFrom(src => src.drr_reasonfordelay.HasValue ? (int?)Enum.Parse<DelayReason>(((DelayReasonOptionSet)src.drr_reasonfordelay).ToString()) : null))
                .ForPath(dest => dest.Workplan.OtherDelayReason, opt => opt.MapFrom(src => src.drr_otherreasonfordelay))
                .ForPath(dest => dest.Workplan.BehindScheduleMitigatingComments, opt => opt.MapFrom(src => src.drr_mitigatingstepstakentoremoveorreducedelay))
                .ForPath(dest => dest.Workplan.ProjectCompletionPercentage, opt => opt.MapFrom(src => src.drr_percentageofprojectcompleteasofreportdate))
                .ForPath(dest => dest.Workplan.ConstructionCompletionPercentage, opt => opt.MapFrom(src => src.drr_percentconstructioncompleteatreportdate))
                .ForPath(dest => dest.Workplan.SignageRequired, opt => opt.MapFrom(src => src.drr_temporaryprovincialfundingsignage.HasValue ? src.drr_temporaryprovincialfundingsignage.Value == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForPath(dest => dest.Workplan.SignageNotRequiredComments, opt => opt.MapFrom(src => src.drr_whyissignagenotrequired))
                .ForPath(dest => dest.Workplan.FundingSignage, opt => opt.MapFrom(src => src.drr_drr_projectprogress_drr_temporaryprovincialfundingsignage_ProjectProgress.Where(c => c.statecode == (int)EntityState.Active)))
                .ForPath(dest => dest.Workplan.MediaAnnouncement, opt => opt.MapFrom(src => src.drr_mediaannouncements.HasValue ? src.drr_mediaannouncements.Value == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForPath(dest => dest.Workplan.MediaAnnouncementDate, opt => opt.MapFrom(src => src.drr_dateofannouncement.HasValue ? src.drr_dateofannouncement.Value.UtcDateTime : (DateTime?)null))
                .ForPath(dest => dest.Workplan.MediaAnnouncementComment, opt => opt.MapFrom(src => src.drr_describeannouncement))
                .ForPath(dest => dest.Workplan.OutstandingIssues, opt => opt.MapFrom(src => src.drr_outstandingissuesyesno.HasValue ? src.drr_outstandingissuesyesno.Value == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForPath(dest => dest.Workplan.OutstandingIssuesComments, opt => opt.MapFrom(src => src.drr_outstandingissues))
                .ForPath(dest => dest.Workplan.FundingSourcesChanged, opt => opt.MapFrom(src => src.drr_changestofundingsources.HasValue ? src.drr_changestofundingsources.Value == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForPath(dest => dest.Workplan.FundingSourcesChangedComment, opt => opt.MapFrom(src => src.drr_commentschangestofundingsources))
                .ForPath(dest => dest.EventInformation.PastEvents, opt => opt.MapFrom(src => src.drr_drr_projectprogress_drr_projectevent_ProjectProgress.Where(c => c.statecode == (int)EntityState.Active)))
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.bcgov_drr_projectprogress_bcgov_documenturl_ProgressReport.Where(c => c.statecode == (int)EntityState.Active)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.statuscode.HasValue ? (int?)Enum.Parse<ProgressReportStatus>(((ProjectProgressReportStatusOptionSet)src.statuscode).ToString()) : null))
            ;
#pragma warning restore CS8604 // Possible null reference argument.

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
                .ForMember(dest => dest.drr_projectworkplanactivityid, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Id) ? Guid.Parse(src.Id) : (Guid?)null))
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.ActivityType.Name))
                .ForMember(dest => dest.drr_plannedstartdate, opt => opt.MapFrom(src => src.PlannedStartDate.HasValue ? src.PlannedStartDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_plannedcompletiondate, opt => opt.MapFrom(src => src.PlannedCompletionDate.HasValue ? src.PlannedCompletionDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_actualstartdate, opt => opt.MapFrom(src => src.ActualStartDate.HasValue ? src.ActualStartDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_actualcompletiondate, opt => opt.MapFrom(src => src.ActualCompletionDate.HasValue ? src.ActualCompletionDate.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_constructioncontractawardstatus, opt => opt.MapFrom(src => src.ConstructionContractStatus.HasValue ? (int?)Enum.Parse<ConstructionContractOptionSet>(src.ConstructionContractStatus.Value.ToString()) : null))
                .ForMember(dest => dest.drr_permittoconstructstatus, opt => opt.MapFrom(src => src.PermitToConstructStatus.HasValue ? (int?)Enum.Parse<PermitToConstructOptionSet>(src.PermitToConstructStatus.Value.ToString()) : null))
                .ForMember(dest => dest.drr_progressstatus, opt => opt.MapFrom(src => src.ProgressStatus.HasValue ? (int?)Enum.Parse<WorkplanProgressOptionSet>(src.ProgressStatus.Value.ToString()) : null))
                .ForMember(dest => dest.drr_ActivityType, opt => opt.MapFrom(src => src.ActivityType))
                .ForMember(dest => dest.drr_explaindatechange, opt => opt.MapFrom(src => src.Comment))
            ;

            CreateMap<drr_projectworkplanactivity, WorkplanActivityDetails>(MemberList.None)
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_projectworkplanactivityid.ToString()))
                .ForMember(dest => dest.PlannedStartDate, opt => opt.MapFrom(src => src.drr_plannedstartdate.HasValue ? src.drr_plannedstartdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.PlannedCompletionDate, opt => opt.MapFrom(src => src.drr_plannedcompletiondate.HasValue ? src.drr_plannedcompletiondate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ActualStartDate, opt => opt.MapFrom(src => src.drr_actualstartdate.HasValue ? src.drr_actualstartdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ActualCompletionDate, opt => opt.MapFrom(src => src.drr_actualcompletiondate.HasValue ? src.drr_actualcompletiondate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ConstructionContractStatus, opt => opt.MapFrom(src => src.drr_constructioncontractawardstatus.HasValue ? (int?)Enum.Parse<ConstructionContractStatus>(((ConstructionContractOptionSet)src.drr_constructioncontractawardstatus).ToString()) : null))
                .ForMember(dest => dest.PermitToConstructStatus, opt => opt.MapFrom(src => src.drr_permittoconstructstatus.HasValue ? (int?)Enum.Parse<PermitToConstructStatus>(((PermitToConstructOptionSet)src.drr_permittoconstructstatus).ToString()) : null))
                .ForMember(dest => dest.ProgressStatus, opt => opt.MapFrom(src => src.drr_progressstatus.HasValue ? (int?)Enum.Parse<WorkplanProgress>(((WorkplanProgressOptionSet)src.drr_progressstatus).ToString()) : null))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.drr_ActivityType))
                .ForMember(dest => dest.OriginalReportId, opt => opt.MapFrom(src => src.drr_CopiedfromReport.drr_projectprogressid.ToString()))
                .ForMember(dest => dest.CopiedFromActivity, opt => opt.MapFrom(src => src.drr_copiedactivity.HasValue ? src.drr_copiedactivity.Value == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.drr_explaindatechange))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.statuscode.HasValue ? (int?)Enum.Parse<WorkplanStatus>(((WorkplanStatusOptionSet)src.statuscode).ToString()) : null))
            ;

            CreateMap<ActivityType, drr_projectactivity>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Name))
            ;

            CreateMap<drr_projectactivity, ActivityType>(MemberList.None)
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.PreCreatedActivity, opt => opt.MapFrom(src => src.drr_precreatedactivity.HasValue ? src.drr_precreatedactivity == (int)DRRTwoOptions.Yes : false))
            ;

            CreateMap<FundingSignage, drr_temporaryprovincialfundingsignage>(MemberList.None)
                .ForMember(dest => dest.drr_temporaryprovincialfundingsignageid, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.drr_typeofsignage, opt => opt.MapFrom(src => src.Type.HasValue ? (int?)Enum.Parse<SignageTypeOptionSet>(src.Type.Value.ToString()) : null))
                .ForMember(dest => dest.drr_dateinstalled, opt => opt.MapFrom(src => src.DateInstalled.HasValue ? src.DateInstalled.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_dateremoved, opt => opt.MapFrom(src => src.DateRemoved.HasValue ? src.DateRemoved.Value.ToUniversalTime() : (DateTimeOffset?)null))
                .ForMember(dest => dest.drr_hasthesignagebeenapprovedbytheprovince, opt => opt.MapFrom(src => src.BeenApproved.HasValue ? src.BeenApproved.Value ? (int?)DRRTwoOptions.Yes : (int?)DRRTwoOptions.No : null))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_temporaryprovincialfundingsignageid))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.drr_typeofsignage.HasValue ? (int?)Enum.Parse<SignageType>(((SignageTypeOptionSet)src.drr_typeofsignage).ToString()) : null))
                .ForMember(dest => dest.DateInstalled, opt => opt.MapFrom(src => src.drr_dateinstalled.HasValue ? src.drr_dateinstalled.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.DateRemoved, opt => opt.MapFrom(src => src.drr_dateremoved.HasValue ? src.drr_dateremoved.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.BeenApproved, opt => opt.MapFrom(src => src.drr_hasthesignagebeenapprovedbytheprovince.HasValue ? src.drr_hasthesignagebeenapprovedbytheprovince.Value == (int)DRRTwoOptions.Yes : (bool?)null))
            ;

            CreateMap<ProjectEventDetails, drr_projectevent>(MemberList.None)
                .ForMember(dest => dest.drr_projecteventid, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Id) ? Guid.Parse(src.Id) : (Guid?)null))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_projecteventid))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.drr_eventtype.HasValue ? (int?)Enum.Parse<EventType>(((EventTypeOptionSet)src.drr_eventtype).ToString()) : null))
                .ForMember(dest => dest.PlannedEventDate, opt => opt.MapFrom(src => src.drr_plannedeventdate.HasValue ? src.drr_plannedeventdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ActualEventDate, opt => opt.MapFrom(src => src.drr_actualeventdate.HasValue ? src.drr_actualeventdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.NextEventDescription, opt => opt.MapFrom(src => src.drr_describenextevent))
                .ForMember(dest => dest.EventContact, opt => opt.MapFrom(src => src.drr_EventContact))
                .ForMember(dest => dest.ProvincialRepresentativeRequest, opt => opt.MapFrom(src => src.drr_provincialrepresentativerequest.HasValue ? src.drr_provincialrepresentativerequest == (int)DRRTwoOptions.Yes : (bool?)null))
                .ForMember(dest => dest.ProvincialRepresentativeRequestComment, opt => opt.MapFrom(src => src.drr_commentsprovincialrepresentativerequest))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.statuscode.HasValue ? (int?)Enum.Parse<EventStatus>(((EventStatusOptionSet)src.statuscode).ToString()) : null))
            ;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8629 // Nullable value type may be null.
        }
    }
}
