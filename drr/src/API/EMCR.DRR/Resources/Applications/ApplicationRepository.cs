using System.Text.RegularExpressions;
using AutoMapper;
using EMCR.DRR.API.Services;
using EMCR.DRR.Dynamics;
using EMCR.DRR.Managers.Intake;
using EMCR.Utilities.Extensions;
using Microsoft.Dynamics.CRM;
using Xrm.Tools.WebAPI;
using Xrm.Tools.WebAPI.Requests;

namespace EMCR.DRR.Resources.Applications
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;
        private readonly CRMWebAPI api;

        public ApplicationRepository(IDRRContextFactory dRRContextFactory, IMapper mapper, IServiceProvider services)
        {
            this.mapper = mapper;
            this.dRRContextFactory = dRRContextFactory;
            using (var scope = services.CreateScope())
            {
                api = scope.ServiceProvider.GetRequiredService<CRMWebAPI>();
            }
        }

        public async Task<ManageApplicationCommandResult> Manage(ManageApplicationCommand cmd)
        {
            return cmd switch
            {
                SubmitApplication c => await HandleSubmitApplication(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ApplicationQueryResult> QueryList(ApplicationQuery query)
        {
            return query switch
            {
                ApplicationsQuery q => await HandleQueryApplicationList(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        public async Task<ApplicationQueryResult> Query(ApplicationQuery query)
        {
            return query switch
            {
                ApplicationsQuery q => await HandleQueryApplication(q),
                _ => throw new NotSupportedException($"{query.GetType().Name} is not supported")
            };
        }

        public async Task<DeclarationQueryResult> Query(DeclarationQuery query)
        {
            var readCtx = dRRContextFactory.CreateReadOnly();

            var results = await readCtx.drr_legaldeclarations.Expand(d => d.drr_ApplicationType).Where(d => d.statecode == (int)EntityState.Active).GetAllPagesAsync();
            var items = mapper.Map<IEnumerable<DeclarationInfo>>(results);
            return new DeclarationQueryResult { Items = items };
        }

        public async Task<EntitiesQueryResult> Query(EntitiesQuery query)
        {
            var readCtx = dRRContextFactory.CreateReadOnly();

            var standardsInfo = (await readCtx.drr_provincialstandards.Expand(d => d.drr_Category).Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => new StandardSingle { Name = d.drr_name, Category = d.drr_Category.drr_name }).ToList();
            var standardCategories = (await readCtx.drr_provincialstandardcategories.Where(d => d.statecode == (int)EntityState.Active).GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var complexityRisks = (await readCtx.drr_projectcomplexityrisks.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var needIdentifications = (await readCtx.drr_projectneedidentifications.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var affectedParties = (await readCtx.drr_impactedoraffectedparties.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var capacityRisks = (await readCtx.drr_projectcapacitychallenges.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var readinessRisks = (await readCtx.drr_projectreadinessrisks.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var sensitivityRisks = (await readCtx.drr_projectsensitivityrisks.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var costConsiderations = (await readCtx.drr_costconsiderations.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var costReductions = (await readCtx.drr_costreductions.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var coBenefits = (await readCtx.drr_cobenefits.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var fiscalYears = (await readCtx.drr_fiscalyears.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var qualifiedProfessionals = (await readCtx.drr_qualifiedprofessionals.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();
            var resiliencies = (await readCtx.drr_resiliencies.Where(d => d.statecode == (int)EntityState.Active && d.drr_name != "Other").GetAllPagesAsync()).Select(d => d.drr_name).ToList();

            var standards = new List<Controllers.StandardInfo>();
            foreach (var category in standardCategories)
            {
                var currStandards = standardsInfo.Where(s => s.Category == category).Select(s => s.Name);
                standards.Add(new Controllers.StandardInfo
                {
                    Category = category,
                    Standards = currStandards
                });
            }

            return new EntitiesQueryResult
            {
                VerificationMethods = needIdentifications,
                AffectedParties = affectedParties,
                Standards = standards,
                CostReductions = costReductions,
                CoBenefits = coBenefits,
                ComplexityRisks = complexityRisks,
                ReadinessRisks = readinessRisks,
                SensitivityRisks = sensitivityRisks,
                CostConsiderations = costConsiderations,
                CapacityRisks = capacityRisks,
                FiscalYears = fiscalYears,
                Professionals = qualifiedProfessionals,
                IncreasedResiliency = resiliencies
            };
        }

        public async Task<bool> CanAccessApplication(string id, string businessId)
        {
            var readCtx = dRRContextFactory.CreateReadOnly();
            var existingApplication = await readCtx.drr_applications.Expand(a => a.drr_Primary_Proponent_Name).Where(a => a.drr_name == id).SingleOrDefaultAsync();
            if (existingApplication == null) return true;
            return (!string.IsNullOrEmpty(existingApplication.drr_Primary_Proponent_Name.drr_bceidguid)) && existingApplication.drr_Primary_Proponent_Name.drr_bceidguid.Equals(businessId);
        }

        private async Task<ApplicationQueryResult> HandleQueryApplicationList(ApplicationsQuery query)
        {
            var filterString = GetFilterXML(query);
            var orderString = GetOrderXML(query);
            var orderByStatus = !string.IsNullOrEmpty(query.OrderBy) && query.OrderBy.Equals("statuscode");

            //If not order by status then perform paging in the query
            var fetchString = orderByStatus ? "<fetch>" : $"<fetch count='{query.Count}' page='{query.Page}'>";
            var fetchXML = @$"{fetchString}
                      <entity name='drr_application'>
                        <attribute name='drr_applicationid' />
                        <attribute name='drr_name' />
                        <attribute name='drr_applicationtypename' />
                        <attribute name='drr_programname' />
                        <attribute name='drr_fullproposalapplication' />
                        <attribute name='drr_projecttitle' />
                        <attribute name='statuscode' />
                        <attribute name='drr_estimateddriffundingprogramrequest' />
                        <attribute name='modifiedon' />
                        <attribute name='drr_submitteddate' />
                        <attribute name='drr_fundingstream' />
                        {filterString}
                        {orderString}
                      </entity>
                    </fetch>";

            var fetchResults = (await api.GetList<drr_application>("drr_applications", new CRMGetListOptions
            {
                FetchXml = fetchXML,
                IncludeCount = true,
            }));

            var length = fetchResults.Count;
            var results = fetchResults.List;

            //CRM statuses have different names than portal ones
            //So sorting by CRM status gives the wrong order in portal
            //So in that case, we query the full results to a small performance penalty, then do a custom sort and skip/take ourselves
            //Hack workaround to save as much performance as we can when we can
            if (!string.IsNullOrEmpty(query.OrderBy) && orderByStatus)
            {
                results = SortAndPageResults(results, query);
            }

            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();
            await Parallel.ForEachAsync(results, ct, async (a, ct) => await ParallelLoadApplicationAsync(readCtx, true, a, ct));
            return new ApplicationQueryResult { Items = mapper.Map<IEnumerable<Application>>(results), Length = length };
        }

        private async Task<ApplicationQueryResult> HandleQueryApplication(ApplicationsQuery query)
        {
            var ct = new CancellationTokenSource().Token;
            var readCtx = dRRContextFactory.CreateReadOnly();

            var applicationsQuery = readCtx.drr_applications
                .Expand(a => a.drr_ApplicationType).Expand(a => a.drr_Program) //Added for sorting purposes
                .Where(a => a.statecode == (int)EntityState.Active);
            if (!string.IsNullOrEmpty(query.Id)) applicationsQuery = applicationsQuery.Where(a => a.drr_name == query.Id);
            if (!string.IsNullOrEmpty(query.BusinessId)) applicationsQuery = applicationsQuery.Where(a => a.drr_Primary_Proponent_Name.drr_bceidguid == query.BusinessId);
            if (query.FilterOptions != null)
            {
                if (!string.IsNullOrEmpty(query.FilterOptions.ApplicationType)) applicationsQuery = applicationsQuery.Where(a => a.drr_ApplicationType.drr_name == query.FilterOptions.ApplicationType);
                if (!string.IsNullOrEmpty(query.FilterOptions.ProgramType)) applicationsQuery = applicationsQuery.Where(a => a.drr_Program.drr_name == query.FilterOptions.ProgramType);
#pragma warning disable CS8629 // Nullable value type may be null.
                if (query.FilterOptions.Statuses != null) applicationsQuery = applicationsQuery.WhereIn(a => a.statuscode.Value, query.FilterOptions.Statuses);
#pragma warning restore CS8629 // Nullable value type may be null.
            }

            var results = (await applicationsQuery.GetAllPagesAsync(ct)).ToArray();
            var length = results.Length;

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                var descending = false;
                if (query.OrderBy.Contains(" desc"))
                {
                    descending = true;
                    query.OrderBy = Regex.Replace(query.OrderBy, @" desc", "");
                }
                if (descending) results = results.OrderByDescending(a => GetPropertyValueForSort(a, query.OrderBy)).ToArray();
                else results = results.OrderBy(a => GetPropertyValueForSort(a, query.OrderBy)).ToArray();
            }
            else
            {
                results = results.OrderBy(a => a.drr_name).ToArray();
            }

            if (query.Page > 0)
            {
                results = results.Skip(query.Page).ToArray();
            }

            if (query.Count > 0)
            {
                results = results.Take(query.Count).ToArray();
            }

            var partnerProponentsOnly = false;
            if (string.IsNullOrEmpty(query.Id)) partnerProponentsOnly = true;

            await Parallel.ForEachAsync(results, ct, async (a, ct) => await ParallelLoadApplicationAsync(readCtx, partnerProponentsOnly, a, ct));
            return new ApplicationQueryResult { Items = mapper.Map<IEnumerable<Application>>(results), Length = length };
        }

        public async Task<ManageApplicationCommandResult> HandleSubmitApplication(SubmitApplication cmd)
        {
            var ctx = dRRContextFactory.Create();
            if (string.IsNullOrEmpty(cmd.Application.Id))
            {
                return new ManageApplicationCommandResult { Id = await Create(ctx, cmd.Application) };
            }
            else
            {
                return new ManageApplicationCommandResult { Id = await Update(ctx, cmd.Application) };
            }
        }

        private async Task<string> Create(DRRContext ctx, Application application)
        {
            var drrApplication = mapper.Map<drr_application>(application);
            drrApplication.drr_applicationid = Guid.NewGuid();
            ctx.AddTodrr_applications(drrApplication);
            return await SaveApplication(ctx, drrApplication, application);
        }

        private async Task<string> Update(DRRContext ctx, Application application)
        {
            var currentApplication = await ctx.drr_applications
                .Where(a => a.drr_name == application.Id)
                .SingleOrDefaultAsync();

            var loadTasks = new List<Task>
            {
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_Program)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_Primary_Proponent_Name)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_SubmitterContact)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_PrimaryProjectContact)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_AdditionalContact1)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_AdditionalContact2)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_application_contact_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_application_fundingsource_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_criticalinfrastructureimpacted_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_qualifiedprofessionalitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_proposedactivity_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_provincialstandarditem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_impactedoraffectedpartyitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_projectneedidentificationitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_costreductionitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_cobenefititem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_projectcomplexityriskitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_projectreadinessriskitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_projectsensitivityriskitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_projectcapacitychallengeitem_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_driffundingrequest_Application)),
                ctx.LoadPropertyAsync(currentApplication, nameof(drr_application.drr_drr_application_drr_resiliencyitem_Application)),
            };

            await Task.WhenAll(loadTasks);

            if (currentApplication == null) throw new NotFoundException("Application not found");

            var partnerAccounts = (await ctx.connections.Expand(c => c.record2id_account).Where(c => c.record1id_drr_application.drr_applicationid == currentApplication.drr_applicationid).GetAllPagesAsync()).Select(p => p.record2id_account).ToList();
            ctx.DetachAll();
            RemoveOldData(ctx, currentApplication, partnerAccounts);

            var drrApplication = mapper.Map<drr_application>(application);
            drrApplication.drr_applicationid = currentApplication.drr_applicationid;

            ctx.AttachTo(nameof(ctx.drr_applications), drrApplication);
            ctx.UpdateObject(drrApplication);
            return await SaveApplication(ctx, drrApplication, application);
        }

        private void RemoveOldData(DRRContext ctx, drr_application drrApplication, IEnumerable<account> partnerAccounts)
        {
            foreach (var account in partnerAccounts)
            {
                ctx.AttachTo(nameof(ctx.accounts), account);
                ctx.DeleteObject(account);
            }

            if (drrApplication.drr_PrimaryProjectContact != null)
            {
                ctx.AttachTo(nameof(ctx.contacts), drrApplication.drr_PrimaryProjectContact);
                ctx.DeleteObject(drrApplication.drr_PrimaryProjectContact);
            }
            if (drrApplication.drr_AdditionalContact1 != null)
            {
                ctx.AttachTo(nameof(ctx.contacts), drrApplication.drr_AdditionalContact1);
                ctx.DeleteObject(drrApplication.drr_AdditionalContact1);
            }
            if (drrApplication.drr_AdditionalContact2 != null)
            {
                ctx.AttachTo(nameof(ctx.contacts), drrApplication.drr_AdditionalContact2);
                ctx.DeleteObject(drrApplication.drr_AdditionalContact2);
            }
            foreach (var fund in drrApplication.drr_application_fundingsource_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_fundingsources), fund);
                ctx.DeleteObject(fund);
            }
            foreach (var infrastructure in drrApplication.drr_drr_application_drr_criticalinfrastructureimpacted_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_criticalinfrastructureimpacteds), infrastructure);
                ctx.DeleteObject(infrastructure);
            }
            foreach (var professional in drrApplication.drr_drr_application_drr_qualifiedprofessionalitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_qualifiedprofessionalitems), professional);
                ctx.DeleteObject(professional);
            }
            foreach (var standard in drrApplication.drr_drr_application_drr_provincialstandarditem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_provincialstandarditems), standard);
                ctx.DeleteObject(standard);
            }
            foreach (var activity in drrApplication.drr_drr_application_drr_proposedactivity_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_proposedactivities), activity);
                ctx.DeleteObject(activity);
            }
            foreach (var need in drrApplication.drr_drr_application_drr_projectneedidentificationitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_projectneedidentificationitems), need);
                ctx.DeleteObject(need);
            }
            foreach (var affectedParty in drrApplication.drr_drr_application_drr_impactedoraffectedpartyitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_impactedoraffectedpartyitems), affectedParty);
                ctx.DeleteObject(affectedParty);
            }
            foreach (var costReduction in drrApplication.drr_drr_application_drr_costreductionitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_costreductionitems), costReduction);
                ctx.DeleteObject(costReduction);
            }
            foreach (var coBenefit in drrApplication.drr_drr_application_drr_cobenefititem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_cobenefititems), coBenefit);
                ctx.DeleteObject(coBenefit);
            }
            foreach (var risk in drrApplication.drr_drr_application_drr_projectcomplexityriskitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_projectcomplexityriskitems), risk);
                ctx.DeleteObject(risk);
            }
            foreach (var risk in drrApplication.drr_drr_application_drr_projectreadinessriskitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_projectreadinessriskitems), risk);
                ctx.DeleteObject(risk);
            }
            foreach (var resiliency in drrApplication.drr_drr_application_drr_resiliencyitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_resiliencyitems), resiliency);
                ctx.DeleteObject(resiliency);
            }
            foreach (var risk in drrApplication.drr_drr_application_drr_projectsensitivityriskitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_projectsensitivityriskitems), risk);
                ctx.DeleteObject(risk);
            }
            foreach (var risk in drrApplication.drr_drr_application_drr_projectcapacitychallengeitem_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_projectcapacitychallengeitems), risk);
                ctx.DeleteObject(risk);
            }
            foreach (var funding in drrApplication.drr_drr_application_drr_driffundingrequest_Application)
            {
                ctx.AttachTo(nameof(ctx.drr_driffundingrequests), funding);
                ctx.DeleteObject(funding);
            }
        }

        private async Task<string> SaveApplication(DRRContext ctx, drr_application drrApplication, Application application)
        {
            var primaryProponent = drrApplication.drr_Primary_Proponent_Name;
            primaryProponent = await CheckForExistingProponent(ctx, primaryProponent, application);

            var submitter = drrApplication.drr_SubmitterContact;
            var primaryProjectContact = drrApplication.drr_PrimaryProjectContact;
            var additionalContact1 = drrApplication.drr_AdditionalContact1;
            var additionalContact2 = drrApplication.drr_AdditionalContact2;

            AssignPrimaryProponent(ctx, drrApplication, primaryProponent);
            if (submitter != null) AddSubmitter(ctx, drrApplication, submitter);
            if (primaryProjectContact != null) AddPrimaryProjectContact(ctx, drrApplication, primaryProjectContact);
            if (additionalContact1 != null) AddAdditionalContact1(ctx, drrApplication, additionalContact1);
            if (additionalContact2 != null) AddAdditionalContact2(ctx, drrApplication, additionalContact2);
            AddFundinSources(ctx, drrApplication);
            AddInfrastructureImpacted(ctx, drrApplication);

            var standardsMasterList = drrApplication.drr_drr_application_drr_provincialstandarditem_Application.Count > 0 ?
                (await ctx.drr_provincialstandards.Expand(s => s.drr_Category).GetAllPagesAsync()).ToList() :
                new List<drr_provincialstandard>();

            var categoryMasterList = drrApplication.drr_drr_application_drr_provincialstandarditem_Application.Count > 0 ?
                (await ctx.drr_provincialstandardcategories.GetAllPagesAsync()).ToList() :
                new List<drr_provincialstandardcategory>();

            var affectedPartiesMasterList = drrApplication.drr_drr_application_drr_impactedoraffectedpartyitem_Application.Count > 0 ?
                (await ctx.drr_impactedoraffectedparties.GetAllPagesAsync()).ToList() :
                new List<drr_impactedoraffectedparty>();

            var projectNeedsMasterList = drrApplication.drr_drr_application_drr_projectneedidentificationitem_Application.Count > 0 ?
                (await ctx.drr_projectneedidentifications.GetAllPagesAsync()).ToList() :
                new List<drr_projectneedidentification>();

            var costReductionsMasterList = drrApplication.drr_drr_application_drr_costreductionitem_Application.Count > 0 ?
                (await ctx.drr_costreductions.GetAllPagesAsync()).ToList() :
                new List<drr_costreduction>();

            var coBenefitsMasterList = drrApplication.drr_drr_application_drr_cobenefititem_Application.Count > 0 ?
                (await ctx.drr_cobenefits.GetAllPagesAsync()).ToList() :
                new List<drr_cobenefit>();

            var complexityRisksMasterList = drrApplication.drr_drr_application_drr_projectcomplexityriskitem_Application.Count > 0 ?
                (await ctx.drr_projectcomplexityrisks.GetAllPagesAsync()).ToList() :
                new List<drr_projectcomplexityrisk>();

            var readinessRisksMasterList = drrApplication.drr_drr_application_drr_projectreadinessriskitem_Application.Count > 0 ?
                (await ctx.drr_projectreadinessrisks.GetAllPagesAsync()).ToList() :
                new List<drr_projectreadinessrisk>();

            var sensitivityRisksMasterList = drrApplication.drr_drr_application_drr_projectsensitivityriskitem_Application.Count > 0 ?
                (await ctx.drr_projectsensitivityrisks.GetAllPagesAsync()).ToList() :
                new List<drr_projectsensitivityrisk>();

            var capacityRisksMasterList = drrApplication.drr_drr_application_drr_projectcapacitychallengeitem_Application.Count > 0 ?
                (await ctx.drr_projectcapacitychallenges.GetAllPagesAsync()).ToList() :
                new List<drr_projectcapacitychallenge>();

            var fiscalYearsMasterList = drrApplication.drr_drr_application_drr_driffundingrequest_Application.Count > 0 ?
                (await ctx.drr_fiscalyears.GetAllPagesAsync()).ToList() :
                new List<drr_fiscalyear>();

            var professionalsMasterList = drrApplication.drr_drr_application_drr_qualifiedprofessionalitem_Application.Count > 0 ?
                (await ctx.drr_qualifiedprofessionals.GetAllPagesAsync()).ToList() :
                new List<drr_qualifiedprofessional>();

            var resiliencyMasterList = drrApplication.drr_drr_application_drr_resiliencyitem_Application.Count > 0 ?
                (await ctx.drr_resiliencies.GetAllPagesAsync()).ToList() :
                new List<drr_resiliency>();

            AddProvincialStandards(ctx, drrApplication, standardsMasterList, categoryMasterList);
            AddQualifiedProfessionals(ctx, drrApplication, professionalsMasterList);
            AddProposedActivities(ctx, drrApplication);
            AddProjectNeedIdentifications(ctx, drrApplication, projectNeedsMasterList);
            AddAffectedParties(ctx, drrApplication, affectedPartiesMasterList);
            AddCostReductions(ctx, drrApplication, costReductionsMasterList);
            AddCoBenefits(ctx, drrApplication, coBenefitsMasterList);
            AddComplexityRisks(ctx, drrApplication, complexityRisksMasterList);
            AddReadinessRisks(ctx, drrApplication, readinessRisksMasterList);
            AddSensitivityRisks(ctx, drrApplication, sensitivityRisksMasterList);
            AddCapacityRisks(ctx, drrApplication, capacityRisksMasterList);
            AddResiliencies(ctx, drrApplication, resiliencyMasterList);
            AddYearOverYearFunding(ctx, drrApplication, fiscalYearsMasterList);

            SetApplicationType(ctx, drrApplication, application.ApplicationTypeName);
            SetProgram(ctx, drrApplication, application.ProgramName);
            await SetDeclarations(ctx, drrApplication);

            var partnerAccounts = mapper.Map<IEnumerable<account>>(application.PartneringProponents);
            foreach (var account in partnerAccounts)
            {
                ctx.AddToaccounts(account);
            }

            await ctx.SaveChangesAsync();
            if (partnerAccounts.Count() > 0)
            {
                CreatePartnerConnections(ctx, drrApplication, partnerAccounts);
                await ctx.SaveChangesAsync();
            }

            //get the autogenerated application number
            var drrApplicationNumber = ctx.drr_applications.Where(a => a.drr_applicationid == drrApplication.drr_applicationid).Select(a => a.drr_name).Single();

            ctx.DetachAll();
            return drrApplicationNumber;
        }

        private async Task<account> CheckForExistingProponent(DRRContext ctx, account proponent, Application application)
        {
            var existingProponent = string.IsNullOrEmpty(application.BCeIDBusinessId) ? null : await ctx.accounts.Where(a => a.drr_bceidguid == application.BCeIDBusinessId).SingleOrDefaultAsync();
            if (existingProponent == null)
            {
                ctx.AddToaccounts(proponent);
            }
            else
            {
                proponent = existingProponent;
            }
            return proponent;
        }

        private static async Task SetDeclarations(DRRContext drrContext, drr_application application)
        {
            var accuracyDeclaration = (await drrContext.drr_legaldeclarations.Where(d => d.statecode == (int)EntityState.Active && d.drr_declarationtype == (int)DeclarationTypeOptionSet.AccuracyOfInformation).GetAllPagesAsync()).FirstOrDefault();
            var representativeDeclaration = (await drrContext.drr_legaldeclarations.Where(d => d.statecode == (int)EntityState.Active && d.drr_declarationtype == (int)DeclarationTypeOptionSet.AuthorizedRepresentative).GetAllPagesAsync()).FirstOrDefault();

            if (accuracyDeclaration != null)
            {
                drrContext.SetLink(application, nameof(drr_application.drr_AccuracyofInformationDeclaration), accuracyDeclaration);
            }

            if (representativeDeclaration != null)
            {
                drrContext.SetLink(application, nameof(drr_application.drr_AuthorizedRepresentativeDeclaration), representativeDeclaration);
            }
        }

        private static void AssignPrimaryProponent(DRRContext drrContext, drr_application application, account primaryProponent)
        {
            drrContext.AddLink(primaryProponent, nameof(primaryProponent.drr_account_drr_application_PrimaryProponentName), application);
        }

        private static void AddSubmitter(DRRContext drrContext, drr_application application, contact submitter)
        {
            drrContext.AddTocontacts(submitter);
            drrContext.AddLink(submitter, nameof(submitter.drr_contact_drr_application_SubmitterContact), application);
        }

        private static void AddPrimaryProjectContact(DRRContext drrContext, drr_application application, contact primaryProjectContact)
        {
            drrContext.AddTocontacts(primaryProjectContact);
            drrContext.AddLink(primaryProjectContact, nameof(primaryProjectContact.drr_contact_drr_application_PrimaryProjectContact), application);
        }

        private static void AddAdditionalContact1(DRRContext drrContext, drr_application application, contact additionalContact1)
        {
            if (additionalContact1 == null || string.IsNullOrEmpty(additionalContact1.firstname)) return;
            drrContext.AddTocontacts(additionalContact1);
            drrContext.AddLink(additionalContact1, nameof(additionalContact1.drr_contact_drr_application_AdditionalContact1), application);
        }

        private static void AddAdditionalContact2(DRRContext drrContext, drr_application application, contact additionalContact2)
        {
            if (additionalContact2 == null || string.IsNullOrEmpty(additionalContact2.firstname)) return;
            drrContext.AddTocontacts(additionalContact2);
            drrContext.AddLink(additionalContact2, nameof(additionalContact2.drr_contact_drr_application_AdditionalContact2), application);
        }

        private static void CreatePartnerConnections(DRRContext drrContext, drr_application application, IEnumerable<account> partnerAccounts)
        {
            var connectionRole = drrContext.connectionroles.Where(r => r.name == "Partner").SingleOrDefault();
            foreach (var account in partnerAccounts)
            {
                var connection = new connection
                {
                    name = account.name,
                };
                drrContext.AddToconnections(connection);
                drrContext.SetLink(connection, nameof(connection.record2roleid), connectionRole);
                drrContext.SetLink(connection, nameof(connection.record2id_account), account);
                drrContext.SetLink(connection, nameof(connection.record1id_drr_application), application);
            }
        }

        private static void AddFundinSources(DRRContext drrContext, drr_application application)
        {
            foreach (var fund in application.drr_application_fundingsource_Application)
            {
                if (fund != null)
                {
                    drrContext.AddTodrr_fundingsources(fund);
                    drrContext.AddLink(application, nameof(application.drr_application_fundingsource_Application), fund);
                    drrContext.SetLink(fund, nameof(fund.drr_Application), application);
                }
            }
        }

        private static void AddInfrastructureImpacted(DRRContext drrContext, drr_application application)
        {
            foreach (var infrastructure in application.drr_drr_application_drr_criticalinfrastructureimpacted_Application)
            {
                if (infrastructure != null && !string.IsNullOrEmpty(infrastructure.drr_name))
                {
                    drrContext.AddTodrr_criticalinfrastructureimpacteds(infrastructure);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_criticalinfrastructureimpacted_Application), infrastructure);
                    drrContext.SetLink(infrastructure, nameof(infrastructure.drr_Application), application);
                }
            }
        }

        private static void AddQualifiedProfessionals(DRRContext drrContext, drr_application application, List<drr_qualifiedprofessional> professionalsMasterList)
        {
            foreach (var professional in application.drr_drr_application_drr_qualifiedprofessionalitem_Application)
            {
                if (professional != null)
                {
                    var masterVal = professionalsMasterList.FirstOrDefault(s => s.drr_name == professional.drr_QualifiedProfessional?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = professionalsMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        professional.drr_qualifiedprofessionalcomments = professional.drr_QualifiedProfessional?.drr_name;
                    }
                    professional.drr_QualifiedProfessional = masterVal;

                    drrContext.AddTodrr_qualifiedprofessionalitems(professional);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_qualifiedprofessionalitem_Application), professional);
                    drrContext.SetLink(professional, nameof(professional.drr_Application), application);
                    drrContext.SetLink(professional, nameof(professional.drr_QualifiedProfessional), masterVal);
                }
            }
        }

        private static void AddProposedActivities(DRRContext drrContext, drr_application application)
        {
            foreach (var activity in application.drr_drr_application_drr_proposedactivity_Application)
            {
                if (activity != null && !string.IsNullOrEmpty(activity.drr_name))
                {
                    drrContext.AddTodrr_proposedactivities(activity);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_proposedactivity_Application), activity);
                    drrContext.SetLink(activity, nameof(activity.drr_Application), application);
                }
            }
        }

        private static void AddProvincialStandards(DRRContext drrContext, drr_application application, List<drr_provincialstandard> standardsMasterList, List<drr_provincialstandardcategory> categoryMasterList)
        {
            foreach (var standard in application.drr_drr_application_drr_provincialstandarditem_Application)
            {
                if (standard != null)
                {
                    var masterVal = standardsMasterList.FirstOrDefault(s => s.drr_name == standard.drr_ProvincialStandard?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = standardsMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        standard.drr_provincialstandarditemcomments = standard.drr_ProvincialStandard?.drr_name;
                    }
                    standard.drr_ProvincialStandard = masterVal;

                    var categoryMasterVal = categoryMasterList.FirstOrDefault(s => s.drr_name == standard.drr_ProvincialStandardCategory?.drr_name);
                    if (categoryMasterVal == null)
                    {
                        categoryMasterVal = categoryMasterList.FirstOrDefault(s => s.drr_name == "Other");
                    }

                    drrContext.AddTodrr_provincialstandarditems(standard);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_provincialstandarditem_Application), standard);
                    drrContext.SetLink(standard, nameof(standard.drr_Application), application);
                    drrContext.SetLink(standard, nameof(standard.drr_ProvincialStandard), masterVal);
                    drrContext.SetLink(standard, nameof(standard.drr_ProvincialStandardCategory), categoryMasterVal);
                }
            }
        }

        private static void AddAffectedParties(DRRContext drrContext, drr_application application, List<drr_impactedoraffectedparty> affectedPartiesMasterList)
        {
            foreach (var affectedParty in application.drr_drr_application_drr_impactedoraffectedpartyitem_Application)
            {
                if (affectedParty != null)
                {
                    var masterVal = affectedPartiesMasterList.FirstOrDefault(s => s.drr_name == affectedParty.drr_ImpactedorAffectedParty?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = affectedPartiesMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        affectedParty.drr_impactedoraffectedpartycomments = affectedParty.drr_ImpactedorAffectedParty?.drr_name;
                    }
                    affectedParty.drr_ImpactedorAffectedParty = masterVal;

                    drrContext.AddTodrr_impactedoraffectedpartyitems(affectedParty);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_impactedoraffectedpartyitem_Application), affectedParty);
                    drrContext.SetLink(affectedParty, nameof(affectedParty.drr_Application), application);
                    drrContext.SetLink(affectedParty, nameof(affectedParty.drr_ImpactedorAffectedParty), masterVal);
                }
            }
        }

        private static void AddProjectNeedIdentifications(DRRContext drrContext, drr_application application, List<drr_projectneedidentification> projectNeedsMasterList)
        {
            foreach (var need in application.drr_drr_application_drr_projectneedidentificationitem_Application)
            {
                if (need != null)
                {
                    var masterVal = projectNeedsMasterList.FirstOrDefault(s => s.drr_name == need.drr_projectneedidentification?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = projectNeedsMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        need.drr_projectneedidentifiedcomments = need.drr_projectneedidentification?.drr_name;
                    }
                    need.drr_projectneedidentification = masterVal;

                    drrContext.AddTodrr_projectneedidentificationitems(need);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_projectneedidentificationitem_Application), need);
                    drrContext.SetLink(need, nameof(need.drr_Application), application);
                    drrContext.SetLink(need, nameof(need.drr_projectneedidentification), masterVal);
                }
            }
        }

        private static void AddCostReductions(DRRContext drrContext, drr_application application, List<drr_costreduction> costReductionMasterList)
        {
            foreach (var need in application.drr_drr_application_drr_costreductionitem_Application)
            {
                if (need != null)
                {
                    var masterVal = costReductionMasterList.FirstOrDefault(s => s.drr_name == need.drr_CostReduction?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = costReductionMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        need.drr_costreductionitemcomments = need.drr_CostReduction?.drr_name;
                    }
                    need.drr_CostReduction = masterVal;

                    drrContext.AddTodrr_costreductionitems(need);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_costreductionitem_Application), need);
                    drrContext.SetLink(need, nameof(need.drr_Application), application);
                    drrContext.SetLink(need, nameof(need.drr_CostReduction), masterVal);
                }
            }
        }

        private static void AddCoBenefits(DRRContext drrContext, drr_application application, List<drr_cobenefit> coBenefitsMasterList)
        {
            foreach (var benefit in application.drr_drr_application_drr_cobenefititem_Application)
            {
                if (benefit != null)
                {
                    var masterVal = coBenefitsMasterList.FirstOrDefault(s => s.drr_name == benefit.drr_CoBenefit?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = coBenefitsMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        benefit.drr_cobenefitcomments = benefit.drr_CoBenefit?.drr_name;
                    }
                    benefit.drr_CoBenefit = masterVal;

                    drrContext.AddTodrr_cobenefititems(benefit);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_cobenefititem_Application), benefit);
                    drrContext.SetLink(benefit, nameof(benefit.drr_Application), application);
                    drrContext.SetLink(benefit, nameof(benefit.drr_CoBenefit), masterVal);
                }
            }
        }

        private static void AddComplexityRisks(DRRContext drrContext, drr_application application, List<drr_projectcomplexityrisk> complexityRisksMasterList)
        {
            foreach (var risk in application.drr_drr_application_drr_projectcomplexityriskitem_Application)
            {
                if (risk != null)
                {
                    var masterVal = complexityRisksMasterList.FirstOrDefault(s => s.drr_name == risk.drr_ProjectComplexityRisk?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = complexityRisksMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        risk.drr_projectcomplexityriskitemcomments = risk.drr_ProjectComplexityRisk?.drr_name;
                    }
                    risk.drr_ProjectComplexityRisk = masterVal;

                    drrContext.AddTodrr_projectcomplexityriskitems(risk);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_projectcomplexityriskitem_Application), risk);
                    drrContext.SetLink(risk, nameof(risk.drr_Application), application);
                    drrContext.SetLink(risk, nameof(risk.drr_ProjectComplexityRisk), masterVal);
                }
            }
        }

        private static void AddReadinessRisks(DRRContext drrContext, drr_application application, List<drr_projectreadinessrisk> readinessRisksMasterList)
        {
            foreach (var risk in application.drr_drr_application_drr_projectreadinessriskitem_Application)
            {
                if (risk != null)
                {
                    var masterVal = readinessRisksMasterList.FirstOrDefault(s => s.drr_name == risk.drr_ProjectReadinessRisk?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = readinessRisksMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        risk.drr_projectreadinessriskcomments = risk.drr_ProjectReadinessRisk?.drr_name;
                    }
                    risk.drr_ProjectReadinessRisk = masterVal;

                    drrContext.AddTodrr_projectreadinessriskitems(risk);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_projectreadinessriskitem_Application), risk);
                    drrContext.SetLink(risk, nameof(risk.drr_Application), application);
                    drrContext.SetLink(risk, nameof(risk.drr_ProjectReadinessRisk), masterVal);
                }
            }
        }

        private static void AddSensitivityRisks(DRRContext drrContext, drr_application application, List<drr_projectsensitivityrisk> sensitivityRisksMasterList)
        {
            foreach (var risk in application.drr_drr_application_drr_projectsensitivityriskitem_Application)
            {
                if (risk != null)
                {
                    var masterVal = sensitivityRisksMasterList.FirstOrDefault(s => s.drr_name == risk.drr_ProjectSensitivityRisk?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = sensitivityRisksMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        risk.drr_projectsensitivityriskcomments = risk.drr_ProjectSensitivityRisk?.drr_name;
                    }
                    risk.drr_ProjectSensitivityRisk = masterVal;

                    drrContext.AddTodrr_projectsensitivityriskitems(risk);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_projectsensitivityriskitem_Application), risk);
                    drrContext.SetLink(risk, nameof(risk.drr_Application), application);
                    drrContext.SetLink(risk, nameof(risk.drr_ProjectSensitivityRisk), masterVal);
                }
            }
        }

        private static void AddCapacityRisks(DRRContext drrContext, drr_application application, List<drr_projectcapacitychallenge> capacityRisksMasterList)
        {
            foreach (var need in application.drr_drr_application_drr_projectcapacitychallengeitem_Application)
            {
                if (need != null)
                {
                    var masterVal = capacityRisksMasterList.FirstOrDefault(s => s.drr_name == need.drr_ProjectCapacityChallenge?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = capacityRisksMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        need.drr_projectcapacitychallengecomments = need.drr_ProjectCapacityChallenge?.drr_name;
                    }
                    need.drr_ProjectCapacityChallenge = masterVal;

                    drrContext.AddTodrr_projectcapacitychallengeitems(need);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_projectcapacitychallengeitem_Application), need);
                    drrContext.SetLink(need, nameof(need.drr_Application), application);
                    drrContext.SetLink(need, nameof(need.drr_ProjectCapacityChallenge), masterVal);
                }
            }
        }

        private static void AddResiliencies(DRRContext drrContext, drr_application application, List<drr_resiliency> resiliencyRisksMasterList)
        {
            foreach (var resiliency in application.drr_drr_application_drr_resiliencyitem_Application)
            {
                if (resiliency != null)
                {
                    var masterVal = resiliencyRisksMasterList.FirstOrDefault(s => s.drr_name == resiliency.drr_Resiliency?.drr_name);
                    if (masterVal == null)
                    {
                        masterVal = resiliencyRisksMasterList.FirstOrDefault(s => s.drr_name == "Other");
                        resiliency.drr_resiliencycomments = resiliency.drr_Resiliency?.drr_name;
                    }
                    resiliency.drr_Resiliency = masterVal;

                    drrContext.AddTodrr_resiliencyitems(resiliency);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_resiliencyitem_Application), resiliency);
                    drrContext.SetLink(resiliency, nameof(resiliency.drr_Application), application);
                    drrContext.SetLink(resiliency, nameof(resiliency.drr_Resiliency), masterVal);
                }
            }
        }

        private static void AddYearOverYearFunding(DRRContext drrContext, drr_application application, List<drr_fiscalyear> fiscalYearsMasterList)
        {
            foreach (var request in application.drr_drr_application_drr_driffundingrequest_Application)
            {
                if (request != null)
                {
                    var masterVal = fiscalYearsMasterList.FirstOrDefault(s => s.drr_name == request.drr_FiscalYear?.drr_name);
                    if (masterVal == null) request.drr_FiscalYear = null;

                    drrContext.AddTodrr_driffundingrequests(request);
                    drrContext.AddLink(application, nameof(application.drr_drr_application_drr_driffundingrequest_Application), request);
                    drrContext.SetLink(request, nameof(request.drr_Application), application);
                    drrContext.SetLink(request, nameof(request.drr_FiscalYear), masterVal);
                }
            }
        }

        private static void SetApplicationType(DRRContext drrContext, drr_application application, string ApplicationTypeName)
        {
            var applicationType = drrContext.drr_applicationtypes.Where(type => type.drr_name == ApplicationTypeName).SingleOrDefault();
            drrContext.AddLink(applicationType, nameof(applicationType.drr_drr_applicationtype_drr_application_ApplicationType), application);
            drrContext.SetLink(application, nameof(application.drr_ApplicationType), applicationType);
        }

        private static void SetProgram(DRRContext drrContext, drr_application application, string ProgramName)
        {
            var program = drrContext.drr_programs.Where(p => p.drr_name == ProgramName).SingleOrDefault();
            drrContext.AddLink(program, nameof(program.drr_drr_program_drr_application_Program), application);
            drrContext.SetLink(application, nameof(application.drr_Program), program);
        }

        private static async Task ParallelLoadApplicationAsync(DRRContext ctx, bool partnerProponentsOnly, drr_application application, CancellationToken ct)
        {
            ctx.AttachTo(nameof(DRRContext.drr_applications), application);

            var loadTasks = new List<Task>
            {
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_connections1), ct),
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_ApplicationType), ct),
                ctx.LoadPropertyAsync(application, nameof(drr_application.drr_FullProposalApplication), ct),
            };

            if (!partnerProponentsOnly)
            {
                loadTasks = loadTasks.Concat(new List<Task>
                {
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_EOIApplication), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_Program), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_Primary_Proponent_Name), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_SubmitterContact), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_PrimaryProjectContact), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_AdditionalContact1), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_AdditionalContact2), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_contact_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_application_fundingsource_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_criticalinfrastructureimpacted_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_qualifiedprofessionalitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_proposedactivity_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_provincialstandarditem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_impactedoraffectedpartyitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_projectneedidentificationitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_costreductionitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_cobenefititem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_projectcomplexityriskitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_projectreadinessriskitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_projectsensitivityriskitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_projectcapacitychallengeitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_resiliencyitem_Application), ct),
                    ctx.LoadPropertyAsync(application, nameof(drr_application.drr_drr_application_drr_driffundingrequest_Application), ct),
                }).ToList();
            }

            await Task.WhenAll(loadTasks);

            await application.drr_drr_application_drr_provincialstandarditem_Application.ForEachAsync(5, async s =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_provincialstandarditems), s);
                await ctx.LoadPropertyAsync(s, nameof(drr_provincialstandarditem.drr_ProvincialStandard), ct);
                await ctx.LoadPropertyAsync(s, nameof(drr_provincialstandarditem.drr_ProvincialStandardCategory), ct);
            });

            await application.drr_drr_application_drr_driffundingrequest_Application.ForEachAsync(5, async f =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_driffundingrequests), f);
                await ctx.LoadPropertyAsync(f, nameof(drr_driffundingrequest.drr_FiscalYear), ct);
            });

            await application.drr_drr_application_drr_qualifiedprofessionalitem_Application.ForEachAsync(5, async f =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_qualifiedprofessionalitems), f);
                await ctx.LoadPropertyAsync(f, nameof(drr_qualifiedprofessionalitem.drr_QualifiedProfessional), ct);
            });

            await application.drr_drr_application_drr_impactedoraffectedpartyitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_impactedoraffectedpartyitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_impactedoraffectedpartyitem.drr_ImpactedorAffectedParty), ct);
            });

            await application.drr_drr_application_drr_projectneedidentificationitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectneedidentificationitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_projectneedidentificationitem.drr_projectneedidentification), ct);
            });

            await application.drr_drr_application_drr_costreductionitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_costreductionitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_costreductionitem.drr_CostReduction), ct);
            });

            await application.drr_drr_application_drr_cobenefititem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_cobenefititems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_cobenefititem.drr_CoBenefit), ct);
            });

            await application.drr_drr_application_drr_projectcomplexityriskitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectcomplexityriskitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_projectcomplexityriskitem.drr_ProjectComplexityRisk), ct);
            });

            await application.drr_drr_application_drr_projectreadinessriskitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectreadinessriskitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_projectreadinessriskitem.drr_ProjectReadinessRisk), ct);
            });

            await application.drr_drr_application_drr_projectsensitivityriskitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectsensitivityriskitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_projectsensitivityriskitem.drr_ProjectSensitivityRisk), ct);
            });

            await application.drr_drr_application_drr_projectcapacitychallengeitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_projectcapacitychallengeitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_projectcapacitychallengeitem.drr_ProjectCapacityChallenge), ct);
            });

            await application.drr_drr_application_drr_resiliencyitem_Application.ForEachAsync(5, async item =>
            {
                ctx.AttachTo(nameof(DRRContext.drr_resiliencyitems), item);
                await ctx.LoadPropertyAsync(item, nameof(drr_resiliencyitem.drr_Resiliency), ct);
            });
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        private List<drr_application> SortAndPageResults(List<drr_application> applications, ApplicationsQuery query)
        {
            var descending = false;
            if (query.OrderBy.Contains(" desc"))
            {
                descending = true;
                query.OrderBy = Regex.Replace(query.OrderBy, @" desc", "");
            }
            if (descending) applications = applications.OrderByDescending(a => GetPropertyValueForSort(a, query.OrderBy)).ToList();
            else applications = applications.OrderBy(a => GetPropertyValueForSort(a, query.OrderBy)).ToList();

            if (query.Page > 0)
            {
                var skip = query.Count * (query.Page - 1);
                applications = applications.Skip(skip).ToList();
            }

            if (query.Count > 0)
            {
                applications = applications.Take(query.Count).ToList();
            }

            return applications;
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        private string GetFilterXML(ApplicationsQuery query)
        {
            var filterString = $@"<link-entity name='account' to='drr_primary_proponent_name' from='accountid' alias='acc' link-type='inner'>
                                    <filter><condition attribute='drr_bceidguid' operator='eq' value='{query.BusinessId}' /></filter>
                                  </link-entity>";

            if (query.FilterOptions != null)
            {
                filterString += "<filter>";
                if (!string.IsNullOrEmpty(query.FilterOptions.ApplicationType))
                {
                    filterString += $@"<condition attribute='drr_applicationtypename' operator='eq' value='{query.FilterOptions.ApplicationType}' />";
                }
                if (!string.IsNullOrEmpty(query.FilterOptions.ProgramType))
                {
                    filterString += $@"<condition attribute='drr_programname' operator='eq' value='{query.FilterOptions.ProgramType}' />";
                }
                if (query.FilterOptions.Statuses != null && query.FilterOptions.Statuses.Any())
                {
                    filterString += @"<condition attribute='statuscode' operator='in'>";
                    foreach (var status in query.FilterOptions.Statuses)
                    {
                        filterString += $"<value>{status}</value>";
                    }
                    filterString += @"</condition>";
                }
                filterString += "</filter>";
            }

            return filterString;
        }

        private string GetOrderXML(ApplicationsQuery query)
        {
            var orderString = "<order attribute='drr_name' />";
            var orderBy = query.OrderBy;

            if (!string.IsNullOrEmpty(orderBy))
            {
                var descending = false;
                if (orderBy.Contains(" desc"))
                {
                    descending = true;
                    orderBy = Regex.Replace(orderBy, @" desc", "");
                }
                if (descending) orderString = $"<order attribute='{orderBy}' descending='true' />";
                else orderString = $"<order attribute='{orderBy}' />";
            }

            return orderString;
        }

        private static object GetPropertyValueForSort(object src, string propName)
        {


            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValueForSort(GetPropertyValueForSort(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
                if (propName == "statuscode") return GetSortedStatuses(prop.GetValue(src, null).ToString());
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                return prop != null ? prop.GetValue(src, null) : null;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        //return value is ordered alphabetically for what SubmissionPortalStatus will be 
        private static int GetSortedStatuses(string status)
        {
            var statusOption = Enum.Parse<ApplicationStatusOptionSet>(status);
            switch (statusOption)
            {
                case ApplicationStatusOptionSet.Approved:
                    return 0; //Approved

                case ApplicationStatusOptionSet.ApprovedInPrinciple:
                    return 1; //ApprovedInPrinciple

                case ApplicationStatusOptionSet.Closed:
                    return 2; //Closed

                case ApplicationStatusOptionSet.DraftStaff:
                case ApplicationStatusOptionSet.DraftProponent:
                    return 3; //Draft

                case ApplicationStatusOptionSet.Invited:
                    return 4; //EligibleInvited

                case ApplicationStatusOptionSet.InPool:
                    return 5; //EligiblePending

                case ApplicationStatusOptionSet.FPSubmitted:
                    return 6; //FullProposalSubmitted

                case ApplicationStatusOptionSet.Ineligible:
                    return 7; //Ineligible

                case ApplicationStatusOptionSet.Submitted:
                case ApplicationStatusOptionSet.InReview:
                    return 8;//UnderReview

                case ApplicationStatusOptionSet.Withdrawn:
                    return 9; //Withdrawn

                default: return 0;
            }
        }
    }
}
