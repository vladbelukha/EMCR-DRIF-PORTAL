using System;
using System.Linq;
//using EMCR.DRR.Dynamics.Microsoft.Dynamics.CRM;

namespace EMCR.DRR.Dynamics
{
    public class DRRContext : Microsoft.Dynamics.CRM.System
    {
        public DRRContext(Uri serviceRoot) : base(serviceRoot)
        {
        }
    }

    public static class DRRContextExtensions
    {
        //public static dfa_areacommunities? LookupCommunityByName(this DfaContext context, string name)
        //{
        //    if (string.IsNullOrEmpty(name)) return null;
        //    return context.dfa_areacommunitieses.Where(c => c.dfa_name == name).FirstOrDefault();
        //}
    }
}
