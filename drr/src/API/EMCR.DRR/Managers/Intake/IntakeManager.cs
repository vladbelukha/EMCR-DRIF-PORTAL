using AutoMapper;
using EMCR.DRR.Resources.Applications;

namespace EMCR.DRR.Managers.Intake
{
    public class IntakeManager : IIntakeManager
    {
        private readonly IMapper mapper;
        private readonly IApplicationRepository applicationRepository;

        public IntakeManager(IMapper mapper, IApplicationRepository applicationRepository)
        {
            this.mapper = mapper;
            this.applicationRepository = applicationRepository;
        }

        public async Task<IntakeQueryResponse> Handle(IntakeQuery cmd)
        {
            return cmd switch
            {
                DrrApplicationsQuery c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<string> Handle(IntakeCommand cmd)
        {
            return cmd switch
            {
                DrifEoiApplicationCommand c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<IntakeQueryResponse> Handle(DrrApplicationsQuery q)
        {
            var res = await applicationRepository.Query(new ApplicationsQuery { Id = q.Id, BusinessId = q.BusinessId });
            return new IntakeQueryResponse { Items = mapper.Map<IEnumerable<Application>>(res.Items) };
        }

        public async Task<string> Handle(DrifEoiApplicationCommand cmd)
        {
            var application = mapper.Map<Application>(cmd.application);
            application.BusinessBCeID = cmd.BusinessId;
            var id = (await applicationRepository.Manage(new SubmitApplication { Application = application })).Id;
            return id;
        }

        public async Task<DeclarationQueryResult> Handle(DeclarationQuery _)
        {
            var res = await applicationRepository.Query(new Resources.Applications.DeclarationQuery());
            return new DeclarationQueryResult { Items = mapper.Map<IEnumerable<DeclarationInfo>>(res.Items) };
        }
    }
}
