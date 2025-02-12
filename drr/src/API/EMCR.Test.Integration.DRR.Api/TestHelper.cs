using Bogus;
using EMCR.DRR.Controllers;
using EMCR.Tests.Unit.DRR;

namespace EMCR.Tests.Integration
{
    public static class TestHelper
    {
        public static DraftEoiApplication CreateNewTestEOIApplication(ContactDetails? submitter = null)
        {
            var eoi = new Faker<DraftEoiApplication>("en_CA").WithApplicationRules(submitter).Generate();
            return eoi;
        }
    }
}
