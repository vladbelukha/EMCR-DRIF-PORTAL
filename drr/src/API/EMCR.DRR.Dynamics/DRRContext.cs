using System;

namespace EMCR.DRR.Dynamics
{
    public class DRRContext : Microsoft.Dynamics.CRM.System
    {
        public DRRContext(Uri serviceRoot) : base(serviceRoot)
        {
        }
    }
}
