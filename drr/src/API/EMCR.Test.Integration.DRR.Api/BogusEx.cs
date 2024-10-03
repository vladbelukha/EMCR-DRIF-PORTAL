using Bogus;
using EMCR.DRR.Controllers;

namespace EMCR.Tests.Unit.DRR
{
    public static class BogusExtensions
    {
#pragma warning disable CS8629 // Nullable value type may be null.
        public static Faker<DraftEoiApplication> WithApplicationRules(this Faker<DraftEoiApplication> faker, ContactDetails? submitter = null)
        {
            return faker
            .RuleFor(a => a.Id, f => "DRIF-EOI-" + f.Random.Number(1000, 9999).ToString())
            .RuleFor(a => a.Status, f => EMCR.DRR.API.Model.SubmissionPortalStatus.Draft)
            .RuleFor(a => a.ProponentType, f => f.Random.Enum<ProponentType>())
            .RuleFor(a => a.Submitter, f => new Faker<ContactDetails>("en_CA").WithContactDetailsRules().Generate())
            .RuleFor(a => a.ProjectContact, f => new Faker<ContactDetails>("en_CA").WithContactDetailsRules().Generate())
            .RuleFor(a => a.AdditionalContacts, f =>
            {
                var contactFaker = new Faker<ContactDetails>("en_CA");
                return contactFaker.WithContactDetailsRules().GenerateBetween(1, 2);
            })
            .RuleFor(a => a.PartneringProponents, f => Enumerable.Range(0, f.Random.Int(0, 5)).Select(x => f.Company.CompanyName()).ToList())
            .RuleFor(a => a.FundingStream, f => f.Random.Enum<FundingStream>())
            .RuleFor(a => a.ProjectTitle, f => f.Name.JobArea())
            .RuleFor(a => a.Stream, f => f.Random.Enum<ProjectType>())
            .RuleFor(a => a.ScopeStatement, f => f.Lorem.Sentence())
            .RuleFor(a => a.RelatedHazards, f => Enumerable.Range(1, f.Random.Int(1, 8)).Select(x => f.Random.Enum<Hazards>()).Distinct().ToList())
            .RuleFor(a => a.OtherHazardsDescription, f => f.Lorem.Sentence())
            .RuleFor(a => a.StartDate, f => DateTime.UtcNow)
            .RuleFor(a => a.EndDate, f => DateTime.UtcNow.AddDays(f.Random.Number(5, 60)))
            .RuleFor(a => a.EstimatedTotal, f => f.Random.Number(10, 1000) * 1000)
            .RuleFor(a => a.FundingRequest, (f, a) => f.Random.Number(10, (int)a.EstimatedTotal.Value / 1000) * 1000)
            .RuleFor(a => a.HaveOtherFunding, f => f.Random.Bool())
            .RuleFor(a => a.OtherFunding, (f, a) => new Faker<FundingInformation>("en_CA").WithFundingInformationRules((int)a.FundingRequest / 1000).GenerateBetween(0, 6))
            .RuleFor(a => a.IntendToSecureFunding, f => f.Lorem.Sentence())
            .RuleFor(a => a.OwnershipDeclaration, f => f.Random.Bool())
            .RuleFor(a => a.OwnershipDescription, f => f.Lorem.Sentence())
            .RuleFor(a => a.LocationDescription, f => f.Lorem.Sentence())
            .RuleFor(a => a.RationaleForFunding, f => f.Lorem.Sentence())
            .RuleFor(a => a.EstimatedPeopleImpacted, f => f.Random.Enum<EstimatedNumberOfPeople>())
            .RuleFor(a => a.CommunityImpact, f => f.Lorem.Sentence())
            .RuleFor(a => a.InfrastructureImpacted, f => new Faker<InfrastructureImpacted>("en_CA").WithInfrastructureImpactedRules().GenerateBetween(0, 5))
            .RuleFor(a => a.DisasterRiskUnderstanding, f => f.Lorem.Sentence())
            .RuleFor(a => a.AdditionalBackgroundInformation, f => f.Lorem.Sentence())
            .RuleFor(a => a.AddressRisksAndHazards, f => f.Lorem.Sentence())
            .RuleFor(a => a.ProjectDescription, f => f.Lorem.Sentence())
            .RuleFor(a => a.AdditionalSolutionInformation, f => f.Lorem.Sentence())
            .RuleFor(a => a.RationaleForSolution, f => f.Lorem.Sentence())
            .RuleFor(a => a.FirstNationsEngagement, f => f.Lorem.Sentence())
            .RuleFor(a => a.NeighbourEngagement, f => f.Lorem.Sentence())
            .RuleFor(a => a.AdditionalEngagementInformation, f => f.Lorem.Sentence())
            .RuleFor(a => a.ClimateAdaptation, f => f.Lorem.Sentence())
            .RuleFor(a => a.OtherInformation, f => f.Lorem.Sentence())
            ;
        }

        public static Faker<ContactDetails> WithContactDetailsRules(this Faker<ContactDetails> faker)
        {
            return faker
                .RuleFor(f => f.FirstName, f => "autotest-" + f.Person.FirstName)
                .RuleFor(f => f.LastName, f => "autotest-" + f.Person.LastName)
                .RuleFor(f => f.Title, f => f.Name.JobTitle())
                .RuleFor(f => f.Department, f => f.Commerce.Department())
                .RuleFor(f => f.Phone, f => f.Person.Phone)
                .RuleFor(f => f.Email, f => f.Person.Email)
                ;
        }

        public static Faker<FundingInformation> WithFundingInformationRules(this Faker<FundingInformation> faker, int max)
        {
            return faker
                .RuleFor(f => f.Name, f => "autotest-" + f.Person.FirstName)
                .RuleFor(f => f.Type, f => f.Random.Enum<FundingType>())
                .RuleFor(f => f.Amount, f => f.Random.Number(10, max) * 100)
                .RuleFor(f => f.OtherDescription, f => f.Lorem.Sentence())
                ;
        }

        public static Faker<InfrastructureImpacted> WithInfrastructureImpactedRules(this Faker<InfrastructureImpacted> faker)
        {
            return faker
                .RuleFor(f => f.Infrastructure, f => "autotest-" + f.Company.CompanyName())
                .RuleFor(f => f.Impact, f => f.Lorem.Sentence())
                ;
        }
#pragma warning restore CS8629 // Nullable value type may be null.
    }
}
