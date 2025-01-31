using System.Text.RegularExpressions;
using AutoMapper;
using EMCR.DRR.API.Model;
using EMCR.DRR.API.Resources.Cases;
using EMCR.DRR.API.Resources.Documents;
using EMCR.DRR.API.Resources.Projects;
using EMCR.DRR.API.Resources.Reports;
using EMCR.DRR.API.Services;
using EMCR.DRR.API.Services.S3;
using EMCR.DRR.Resources.Applications;
using EMCR.Utilities.Extensions;

namespace EMCR.DRR.Managers.Intake
{
    public class IntakeManager : IIntakeManager
    {
        private readonly ILogger<IntakeManager> logger;
        private readonly IMapper mapper;
        private readonly IApplicationRepository applicationRepository;
        private readonly IProjectRepository projectRepository;
        private readonly IReportRepository reportRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly ICaseRepository caseRepository;
        private readonly IS3Provider s3Provider;

        private FileTag GetDeletedFileTag() => new FileTag { Tags = new[] { new Tag { Key = "Deleted", Value = "true" } } };

        public IntakeManager(ILogger<IntakeManager> logger, IMapper mapper, IApplicationRepository applicationRepository, IDocumentRepository documentRepository, ICaseRepository caseRepository, IProjectRepository projectRepository, IReportRepository reportRepository, IS3Provider s3Provider)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.applicationRepository = applicationRepository;
            this.documentRepository = documentRepository;
            this.caseRepository = caseRepository;
            this.projectRepository = projectRepository;
            this.reportRepository = reportRepository;
            this.s3Provider = s3Provider;
        }

        public async Task<ApplicationQueryResponse> Handle(ApplicationQuery cmd)
        {
            return cmd switch
            {
                DrrApplicationsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ProjectsQueryResponse> Handle(ProjectQuery cmd)
        {
            return cmd switch
            {
                DrrProjectsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ReportsQueryResponse> Handle(ReportQuery cmd)
        {
            return cmd switch
            {
                DrrReportsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ClaimsQueryResponse> Handle(ClaimQuery cmd)
        {
            return cmd switch
            {
                DrrClaimsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ProgressReportsQueryResponse> Handle(ProgressReportQuery cmd)
        {
            return cmd switch
            {
                DrrProgressReportsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ForecastsQueryResponse> Handle(ForecastQuery cmd)
        {
            return cmd switch
            {
                DrrForecastsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<StorageQueryResults> Handle(AttachmentQuery cmd)
        {
            return cmd switch
            {
                DownloadAttachment c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<string> Handle(IntakeCommand cmd)
        {
            return cmd switch
            {
                EoiSaveApplicationCommand c => await Handle(c),
                EoiSubmitApplicationCommand c => await Handle(c),
                CreateFpFromEoiCommand c => await Handle(c),
                FpSaveApplicationCommand c => await Handle(c),
                FpSubmitApplicationCommand c => await Handle(c),
                WithdrawApplicationCommand c => await Handle(c),
                DeleteApplicationCommand c => await Handle(c),
                UploadAttachmentCommand c => await Handle(c),
                DeleteAttachmentCommand c => await Handle(c),
                SaveProjectCommand c => await Handle(c),
                SubmitProjectCommand c => await Handle(c),
                SaveProgressReportCommand c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ApplicationQueryResponse> Handle(DrrApplicationsQuery q)
        {
            if (!string.IsNullOrEmpty(q.Id))
            {
                var canAccess = await CanAccessApplication(q.Id, q.BusinessId);
                if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            }
            var page = 0;
            var count = 0;
            if (q.QueryOptions != null)
            {
                page = q.QueryOptions.Page + 1;
                count = q.QueryOptions.PageSize;
            }

            var orderBy = GetOrderBy(q.QueryOptions?.OrderBy);
            var filterOptions = ParseFilter(q.QueryOptions?.Filter);

            var res = string.IsNullOrEmpty(q.Id) ? await applicationRepository.QueryList(new ApplicationsQuery { Id = q.Id, BusinessId = q.BusinessId, Page = page, Count = count, OrderBy = orderBy, FilterOptions = filterOptions }) :
            await applicationRepository.Query(new ApplicationsQuery { Id = q.Id, BusinessId = q.BusinessId, Page = page, Count = count, OrderBy = orderBy, FilterOptions = filterOptions });

            return new ApplicationQueryResponse { Items = res.Items, Length = res.Length };
        }

        public async Task<ProjectsQueryResponse> Handle(DrrProjectsQuery q)
        {
            if (!string.IsNullOrEmpty(q.Id))
            {
                var canAccess = await CanAccessProject(q.Id, q.BusinessId);
                if (!canAccess) throw new ForbiddenException("Not allowed to access this project.");
            }
            var page = 0;
            var count = 0;
            if (q.QueryOptions != null)
            {
                page = q.QueryOptions.Page + 1;
                count = q.QueryOptions.PageSize;
            }

            var orderBy = GetOrderBy(q.QueryOptions?.OrderBy);
            var filterOptions = ParseFilter(q.QueryOptions?.Filter);

            var res = await projectRepository.Query(new ProjectsQuery { Id = q.Id, BusinessId = q.BusinessId, Page = page, Count = count, OrderBy = orderBy, FilterOptions = filterOptions });

            return new ProjectsQueryResponse { Items = res.Items, Length = res.Length };
        }

        public async Task<ReportsQueryResponse> Handle(DrrReportsQuery q)
        {
            if (!string.IsNullOrEmpty(q.Id))
            {
                var canAccess = await CanAccessClaim(q.Id, q.BusinessId);
                if (!canAccess) throw new ForbiddenException("Not allowed to access this claim.");
            }
            var res = await reportRepository.Query(new ReportsQuery { Id = q.Id, BusinessId = q.BusinessId });

            return new ReportsQueryResponse { Items = res.Items, Length = res.Length };
        }

        public async Task<ClaimsQueryResponse> Handle(DrrClaimsQuery q)
        {
            if (!string.IsNullOrEmpty(q.Id))
            {
                var canAccess = await CanAccessClaim(q.Id, q.BusinessId);
                if (!canAccess) throw new ForbiddenException("Not allowed to access this claim.");
            }
            var res = await reportRepository.Query(new ClaimsQuery { Id = q.Id, BusinessId = q.BusinessId });

            return new ClaimsQueryResponse { Items = res.Items, Length = res.Length };
        }

        public async Task<ProgressReportsQueryResponse> Handle(DrrProgressReportsQuery q)
        {
            if (!string.IsNullOrEmpty(q.Id))
            {
                var canAccess = await CanAccessClaim(q.Id, q.BusinessId);
                if (!canAccess) throw new ForbiddenException("Not allowed to access this progress report.");
            }
            var res = await reportRepository.Query(new ProgressReportsQuery { Id = q.Id, BusinessId = q.BusinessId });

            return new ProgressReportsQueryResponse { Items = res.Items, Length = res.Length };
        }

        public async Task<ForecastsQueryResponse> Handle(DrrForecastsQuery q)
        {
            if (!string.IsNullOrEmpty(q.Id))
            {
                var canAccess = await CanAccessClaim(q.Id, q.BusinessId);
                if (!canAccess) throw new ForbiddenException("Not allowed to access this forecast.");
            }
            var res = await reportRepository.Query(new ForecastsQuery { Id = q.Id, BusinessId = q.BusinessId });

            return new ForecastsQueryResponse { Items = res.Items, Length = res.Length };
        }

        public async Task<string> Handle(EoiSaveApplicationCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.Application.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = mapper.Map<Application>(cmd.Application);
            application.BCeIDBusinessId = cmd.UserInfo.BusinessId;
            application.ProponentName = cmd.UserInfo.BusinessName;
            if (application.Submitter != null) application.Submitter.BCeId = cmd.UserInfo.UserId;
            var id = (await applicationRepository.Manage(new SaveApplication { Application = application })).Id;
            return id;
        }

        public async Task<string> Handle(EoiSubmitApplicationCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.Application.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = mapper.Map<Application>(cmd.Application);
            application.BCeIDBusinessId = cmd.UserInfo.BusinessId;
            application.ProponentName = cmd.UserInfo.BusinessName;
            application.SubmittedDate = DateTime.UtcNow;
            if (application.Submitter != null) application.Submitter.BCeId = cmd.UserInfo.UserId;
            //TODO - add field validations

            var id = (await applicationRepository.Manage(new SaveApplication { Application = application })).Id;
            return id;
        }

        public async Task<string> Handle(CreateFpFromEoiCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.EoiId, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");

            var application = (await applicationRepository.Query(new ApplicationsQuery { Id = cmd.EoiId })).Items.SingleOrDefault();
            if (application == null) throw new NotFoundException("Application not found");
            if (!application.ApplicationTypeName.Equals("EOI")) throw new BusinessValidationException("Can only create FP from an EOI");
            if (application.Status != ApplicationStatus.Invited) throw new BusinessValidationException("Can only create FP if EOI is Invited");
            if (!string.IsNullOrEmpty(application.FpId)) throw new BusinessValidationException("This EOI already has an associated FP");

            var res = (await caseRepository.Manage(new GenerateFpFromEoi { EoiId = cmd.EoiId, ScreenerQuestions = cmd.ScreenerQuestions })).Id;
            return res;
        }

        public async Task<string> Handle(FpSaveApplicationCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.Application.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = mapper.Map<Application>(cmd.Application);
            application.BCeIDBusinessId = cmd.UserInfo.BusinessId;
            application.ProponentName = cmd.UserInfo.BusinessName;
            if (application.Submitter != null) application.Submitter.BCeId = cmd.UserInfo.UserId;
            var id = (await applicationRepository.Manage(new SaveApplication { Application = application })).Id;
            return id;
        }

        public async Task<string> Handle(FpSubmitApplicationCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.Application.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = mapper.Map<Application>(cmd.Application);
            application.BCeIDBusinessId = cmd.UserInfo.BusinessId;
            application.ProponentName = cmd.UserInfo.BusinessName;
            if (application.Submitter != null) application.Submitter.BCeId = cmd.UserInfo.UserId;
            var id = (await applicationRepository.Manage(new SaveApplication { Application = application })).Id;
            await applicationRepository.Manage(new SubmitApplication { Id = id });
            return id;
        }

        public async Task<string> Handle(WithdrawApplicationCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = (await applicationRepository.Query(new ApplicationsQuery { Id = cmd.Id })).Items.SingleOrDefault();
            if (application == null) throw new NotFoundException("Application not found");
            if (application.Status != ApplicationStatus.Submitted) throw new BusinessValidationException("Application can only be withdrawn while it is in Submitted Status");
            application.Status = ApplicationStatus.Withdrawn;
            var id = (await applicationRepository.Manage(new SaveApplication { Application = application })).Id;
            return id;
        }

        public async Task<string> Handle(DeleteApplicationCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = (await applicationRepository.Query(new ApplicationsQuery { Id = cmd.Id })).Items.SingleOrDefault();
            if (application == null) throw new NotFoundException("Application not found");
            if (!ApplicationInEditableStatus(application)) throw new BusinessValidationException("Application can only be deleted if it is in Draft");
            if (!application.ApplicationTypeName.Equals("EOI")) throw new BusinessValidationException("Only EOI applications can be deleted");
            var id = (await applicationRepository.Manage(new DeleteApplication { Id = cmd.Id })).Id;
            return id;
        }

        public async Task<string> Handle(UploadAttachmentCommand cmd)
        {
            var canAccess = await CanAccessApplication(cmd.AttachmentInfo.ApplicationId, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var application = (await applicationRepository.Query(new ApplicationsQuery { Id = cmd.AttachmentInfo.ApplicationId })).Items.SingleOrDefault();
            if (application == null) throw new NotFoundException("Application not found");
            if (!ApplicationInEditableStatus(application)) throw new BusinessValidationException("Can only edit attachments when application is in Draft");
            if (cmd.AttachmentInfo.DocumentType != DocumentType.OtherSupportingDocument && application.Attachments != null && application.Attachments.Any(a => a.DocumentType == cmd.AttachmentInfo.DocumentType))
            {
                throw new BusinessValidationException($"A document with type {cmd.AttachmentInfo.DocumentType.ToDescriptionString()} already exists on the application {cmd.AttachmentInfo.ApplicationId}");
            }

            var newDocId = Guid.NewGuid().ToString();

            await s3Provider.HandleCommand(new UploadFileCommand { Key = newDocId, File = cmd.AttachmentInfo.File, Folder = $"drr_application/{application.CrmId}" });
            var documentRes = (await documentRepository.Manage(new CreateDocument { NewDocId = newDocId, ApplicationId = cmd.AttachmentInfo.ApplicationId, Document = new Document { Name = cmd.AttachmentInfo.File.FileName, DocumentType = cmd.AttachmentInfo.DocumentType, Size = GetFileSize(cmd.AttachmentInfo.File.Content) } }));
            return documentRes.Id;
        }

        public async Task<string> Handle(DeleteAttachmentCommand cmd)
        {
            var canAccess = await CanAccessApplicationFromDocumentId(cmd.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var documentRes = await documentRepository.Manage(new DeleteDocument { Id = cmd.Id });
            await s3Provider.HandleCommand(new UpdateTagsCommand { Key = cmd.Id, Folder = $"drr_application/{documentRes.ApplicationId}", FileTag = GetDeletedFileTag() });
            return documentRes.Id;
        }

        public async Task<string> Handle(SaveProjectCommand cmd)
        {
            var canAccess = await CanAccessProject(cmd.Project.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this project.");
            var project = mapper.Map<Project>(cmd.Project);
            project.ProponentName = cmd.UserInfo.BusinessName;
            //var id = (await projectRepository.Manage(new SaveProject { Project = project })).Id;
            var id = Guid.NewGuid().ToString();
            return id;
        }

        public async Task<string> Handle(SubmitProjectCommand cmd)
        {
            var canAccess = await CanAccessProject(cmd.Project.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this project.");
            var project = mapper.Map<Project>(cmd.Project);
            project.ProponentName = cmd.UserInfo.BusinessName;
            //var id = (await projectRepository.Manage(new SaveProject { Project = project })).Id;
            //await projectRepository.Manage(new SubmitProject { Id = id });
            var id = Guid.NewGuid().ToString();
            return id;
        }
        
        public async Task<string> Handle(SaveProgressReportCommand cmd)
        {
            var canAccess = await CanAccessProgressReport(cmd.ProgressReport.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this progress report.");
            var progressReport = mapper.Map<ProgressReportDetails>(cmd.ProgressReport);
            //progressReport.ProponentName = cmd.UserInfo.BusinessName;
            var id = (await reportRepository.Manage(new SaveProgressReport { ProgressReport = progressReport })).Id;
            //await projectRepository.Manage(new SubmitProject { Id = id });
            //var id = Guid.NewGuid().ToString();
            return id;
        }

        public async Task<FileQueryResult> Handle(DownloadAttachment cmd)
        {
            var canAccess = await CanAccessApplicationFromDocumentId(cmd.Id, cmd.UserInfo.BusinessId);
            if (!canAccess) throw new ForbiddenException("Not allowed to access this application.");
            var applicationId = (await documentRepository.Query(new DocumentQuery { Id = cmd.Id })).ApplicationId;

            var res = await s3Provider.HandleQuery(new FileQuery { Key = cmd.Id, Folder = $"drr_application/{applicationId}" });
            return (FileQueryResult)res;
        }

        public async Task<DeclarationQueryResult> Handle(DeclarationQuery _)
        {
            var res = await applicationRepository.Query(new Resources.Applications.DeclarationQuery());
            return new DeclarationQueryResult { Items = mapper.Map<IEnumerable<DeclarationInfo>>(res.Items) };
        }

        public async Task<EntitiesQueryResult> Handle(EntitiesQuery _)
        {
            var res = await applicationRepository.Query(new Resources.Applications.EntitiesQuery());
            return mapper.Map<EntitiesQueryResult>(res);
        }

        private string GetFileSize(byte[] file)
        {
            float bytes = file.Length;
            if (bytes < 1024) return $"{bytes.ToString("0.00")} B";
            bytes = bytes / 1024f;
            if (bytes < 1024) return $"{bytes.ToString("0.00")} KB";
            bytes = bytes / 1024f;
            if (bytes < 1024) return $"{bytes.ToString("0.00")} MB";
            bytes = bytes / 1024f;
            if (bytes < 1024) return $"{bytes.ToString("0.00")} GB";
            bytes = bytes / 1024f;
            return $"{bytes.ToString("0.00")} TB";
        }

        private bool ApplicationInEditableStatus(Application application)
        {
            return application.Status == ApplicationStatus.DraftProponent || application.Status == ApplicationStatus.DraftStaff || application.Status == ApplicationStatus.Withdrawn;
        }

        private async Task<bool> CanAccessApplication(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            return await applicationRepository.CanAccessApplication(id, businessId);
        }

        private async Task<bool> CanAccessApplicationFromDocumentId(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            return await applicationRepository.CanAccessApplicationFromDocumentId(id, businessId);
        }

        private async Task<bool> CanAccessProject(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            return await projectRepository.CanAccessProject(id, businessId);
        }

        private async Task<bool> CanAccessReport(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            logger.LogDebug("CanAccessReport not implemented");
            //return await reportRepository.CanAccessReport(id, businessId);
            return await Task.FromResult(true);
        }

        private async Task<bool> CanAccessClaim(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            logger.LogDebug("CanAccessClaim not implemented");
            //return await reportRepository.CanAccessClaim(id, businessId);
            return await Task.FromResult(true);
        }

        private async Task<bool> CanAccessProgressReport(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            logger.LogDebug("CanAccessProgressReport not implemented");
            //return await reportRepository.CanAccessProgressReport(id, businessId);
            return await Task.FromResult(true);
        }

        private async Task<bool> CanAccessForecast(string? id, string? businessId)
        {
            if (string.IsNullOrEmpty(businessId)) throw new ArgumentNullException("Missing user's BusinessId");
            if (string.IsNullOrEmpty(id)) return true;
            logger.LogDebug("CanAccessForecast not implemented");
            //return await reportRepository.CanAccessForecast(id, businessId);
            return await Task.FromResult(true);
        }

        private FilterOptions ParseFilter(string? filter)
        {
            if (string.IsNullOrEmpty(filter)) return new FilterOptions();

            var ret = new FilterOptions();

            var parts = filter.Split(',');
            foreach (var part in parts)
            {
                var name = part.Split('=')[0]?.ToLower();
                var value = part.Split("=")[1];

                switch (name)
                {
                    case "programtype":
                        {
                            ret.ProgramType = value.ToUpper();
                            break;
                        }
                    case "applicationtype":
                        {
                            if (value.ToLower() == "fp") value = "Full Proposal";
                            if (value.ToLower() == "eoi") value = "EOI";
                            ret.ApplicationType = value;
                            break;
                        }
                    case "status":
                        {
                            value = Regex.Replace(value, @"\*", "");
                            var selectedStatuses = value.Split("\\|");
                            var statuses = new List<int>();
                            foreach (var currStatus in selectedStatuses)
                            {
                                var submissionStatus = Enum.Parse<SubmissionPortalStatus>(currStatus);
                                var applicationStatuses = IntakeStatusMapper(submissionStatus);
                                statuses = statuses.Concat(applicationStatuses).ToList();
                            }
                            ret.Statuses = statuses;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            return ret;
        }

        private string GetOrderBy(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) return "drr_name desc";
            orderBy = orderBy.ToLower();
            var descending = false;
            if (orderBy.Contains(" desc"))
            {
                descending = true;
                orderBy = Regex.Replace(orderBy, @" desc", "");
                orderBy = Regex.Replace(orderBy, @" asc", "");
            }

            var dir = descending ? " desc" : "";

            switch (orderBy)
            {
                case "id": return "drr_name" + dir;
                case "projecttitle": return "drr_projecttitle" + dir;
                case "applicationtype": return "drr_applicationtypename" + dir;
                case "programtype": return "drr_programname" + dir;
                case "status": return "statuscode" + dir;
                case "fundingrequest": return "drr_eligibleamount" + dir;
                case "modifieddate": return "modifiedon" + dir;
                case "submitteddate": return "drr_submitteddate" + dir;
                default: return "drr_name";
            }
        }

        private List<int> IntakeStatusMapper(SubmissionPortalStatus? status)
        {
            var ret = new List<int>();
            switch (status)
            {
                case SubmissionPortalStatus.Draft:
                    {
                        ret.Add((int)(int)ApplicationStatusOptionSet.DraftStaff);
                        ret.Add((int)ApplicationStatusOptionSet.DraftProponent);
                        break;
                    }
                case SubmissionPortalStatus.UnderReview:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.Submitted);
                        ret.Add((int)ApplicationStatusOptionSet.InReview);
                        break;
                    }
                case SubmissionPortalStatus.EligibleInvited:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.Invited);
                        break;
                    }
                case SubmissionPortalStatus.EligiblePending:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.InPool);
                        break;
                    }
                case SubmissionPortalStatus.Ineligible:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.Ineligible);
                        break;
                    }
                case SubmissionPortalStatus.Withdrawn:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.Withdrawn);
                        break;
                    }
                case SubmissionPortalStatus.Closed:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.Closed);
                        break;
                    }
                case SubmissionPortalStatus.FullProposalSubmitted:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.FPSubmitted);
                        break;
                    }
                case SubmissionPortalStatus.Approved:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.Approved);
                        break;
                    }
                case SubmissionPortalStatus.ApprovedInPrinciple:
                    {
                        ret.Add((int)ApplicationStatusOptionSet.ApprovedInPrinciple);
                        break;
                    }
                default: break;
            }
            return ret;
        }
    }
}
