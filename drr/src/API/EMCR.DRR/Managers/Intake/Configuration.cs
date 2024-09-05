using EMCR.DRR.Managers.Intake;

namespace EMCR.DRR.Managers.Intake
{
    public static class Configuration
    {
        public static IServiceCollection AddIntakeManager(this IServiceCollection services)
        {
            services.AddTransient<IIntakeManager, IntakeManager>();
            return services;
        }
    }
}
