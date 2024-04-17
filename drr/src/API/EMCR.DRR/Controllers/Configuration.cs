using EMCR.DRR.Resources.Applications;

namespace EMCR.DRR.Controllers
{
    public static class Configuration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IApplicationRepository, ApplicationRepository>();
            return services;
        }
    }
}
