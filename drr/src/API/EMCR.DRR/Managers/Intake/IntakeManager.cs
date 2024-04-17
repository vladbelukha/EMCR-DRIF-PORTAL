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

        public async Task<string> Handle(IntakeCommand cmd)
        {
            return cmd switch
            {
                DrifEoiApplicationCommand c => await Handle(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<string> Handle(DrifEoiApplicationCommand cmd)
        {
            var application = mapper.Map<Application>(cmd.application);
            var id = (await applicationRepository.Manage(new SubmitApplication { Application = application })).Id;
            return id;
        }
    }
}
