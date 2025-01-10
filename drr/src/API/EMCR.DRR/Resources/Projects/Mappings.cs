using AutoMapper;
using EMCR.DRR.API.Resources.Projects;
using EMCR.DRR.Managers.Intake;
using EMCR.DRR.Resources.Applications;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Cases
{
    public class ProjectMapperProfile : Profile
    {
        public ProjectMapperProfile()
        {
#pragma warning disable CS8629 // Nullable value type may be null.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            CreateMap<Project, drr_project>(MemberList.None)
                //.ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.EoiId, opt => opt.MapFrom(src => src.drr_Case.drr_EOIApplication.drr_name))
                .ForMember(dest => dest.FpId, opt => opt.MapFrom(src => src.drr_FullProposalApplication.drr_name))
                .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.drr_projectname))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.drr_contractnumber))
                .ForMember(dest => dest.ProponentName, opt => opt.MapFrom(src => src.drr_ProponentName.name))
                .ForMember(dest => dest.FundingStream, opt => opt.MapFrom(src => src.drr_FullProposalApplication.drr_fundingstream.HasValue ? (int?)Enum.Parse<FundingStream>(((FundingStreamOptionSet)src.drr_FullProposalApplication.drr_fundingstream).ToString()) : null))
                //.ForMember(dest => dest.ProjectNumber, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ProjectNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ProgramType, opt => opt.MapFrom(src => src.drr_Program.drr_name))
                .ForMember(dest => dest.ReportingScheduleType, opt => opt.MapFrom(src => src.drr_ReportingSchedule.drr_name))
                //.ForMember(dest => dest.Conditions, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.Conditions, opt => opt.Ignore())
                .ForMember(dest => dest.FundingAmount, opt => opt.MapFrom(src => src.drr_fundingamount))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.drr_plannedstartdate.HasValue ? src.drr_plannedstartdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.drr_plannedenddate.HasValue ? src.drr_plannedenddate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ProjectStatus>(((ProjectStatusOptionSet)src.statuscode).ToString())))
                //.ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.con))
                .ForMember(dest => dest.Contacts, opt => opt.Ignore())
                .ForMember(dest => dest.InterimReports, opt => opt.MapFrom(src => src.drr_drr_project_drr_projectreport_Project))
                .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.drr_drr_project_drr_projectclaim_Project))
                .ForMember(dest => dest.ProgressReports, opt => opt.MapFrom(src => src.drr_drr_project_drr_projectprogress_Project))
                .ForMember(dest => dest.Forecast, opt => opt.MapFrom(src => src.drr_drr_project_drr_projectbudgetforecast_Project))
                .ForMember(dest => dest.Events, opt => opt.MapFrom(src => src.drr_drr_project_drr_projectevent_Project))
                //.ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.doc))
                .ForMember(dest => dest.Attachments, opt => opt.Ignore())
                ;

            CreateMap<InterimReport, drr_projectreport>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.drr_progressdescription))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.drr_reportdate.HasValue ? src.drr_reportdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<InterimReportStatus>(((ProjectReportStatusOptionSet)src.statuscode).ToString())))
                .ForMember(dest => dest.ProjectType, opt => opt.MapFrom(src => src.drr_projecttype.HasValue ? (int?)Enum.Parse<InterimProjectType>(((FundingStreamOptionSet)src.drr_projecttype).ToString()) : null))
                .ForMember(dest => dest.PeriodType, opt => opt.MapFrom(src => src.drr_periodtype.HasValue ? (int?)Enum.Parse<PeriodType>(((PeriodTypeOptionSet)src.drr_periodtype).ToString()) : null))
                .ForMember(dest => dest.Claim, opt => opt.Ignore())
                .ForMember(dest => dest.Report, opt => opt.Ignore())
                .ForMember(dest => dest.Forecast, opt => opt.Ignore())
            ;

            CreateMap<ProjectClaim, drr_projectclaim>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ClaimAmount, opt => opt.MapFrom(src => src.drr_claimamount))
                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src => src.drr_statusdate.HasValue ? src.drr_statusdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ClaimType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
            ;

            CreateMap<ProgressReport, drr_projectprogress>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ReportDate, opt => opt.MapFrom(src => src.drr_reportasatdate.HasValue ? src.drr_reportasatdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.WorkplanActivities, opt => opt.MapFrom(src => src.drr_drr_projectprogress_drr_projectworkplanactivity_ProjectProgressReport))
                .ForMember(dest => dest.ReportType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<ProgressReportStatus>(((ProjectProgressReportStatusOptionSet)src.statuscode).ToString())))
            ;

            CreateMap<Forecast, drr_projectbudgetforecast>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.MapFrom(src => src.Id))
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.drr_name))
                .ForMember(dest => dest.ForecastAmount, opt => opt.MapFrom(src => src.drr_amountspent))
                .ForMember(dest => dest.ForecastDate, opt => opt.MapFrom(src => src.drr_reportasatdate.HasValue ? src.drr_reportasatdate.Value.UtcDateTime : (DateTime?)null))
                .ForMember(dest => dest.ForecastType, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
            ;

            CreateMap<ProjectEvent, drr_projectevent>(MemberList.None)
                .ForMember(dest => dest.drr_name, opt => opt.Ignore())
                .ReverseMap()
                .ValidateMemberList(MemberList.Destination)
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.drr_eventstatus.HasValue ? (int?)Enum.Parse<EventStatus>(((EventStatusOptionSet)src.drr_eventstatus).ToString()) : null))
            ;

            CreateMap<WorkplanActivity, drr_projectworkplanactivity>(MemberList.None)
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
