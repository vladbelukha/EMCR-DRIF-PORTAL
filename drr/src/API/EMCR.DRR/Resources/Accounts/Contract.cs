namespace EMCR.DRR.API.Resources.Accounts
{
    public interface IAccountRepository
    {
        Task<ManageAccountCommandResult> Manage(ManageAccountCommand cmd);
    }

    public abstract class ManageAccountCommand
    { }

    public class SaveAccountIfNotExists : ManageAccountCommand
    {
        public required Account Account { get; set; }
    }

    public class ManageAccountCommandResult
    {
        public required string Id { get; set; }
    }

    public class Account
    {
        public required string Name { get; set; }
        public required string BCeIDBusinessId { get; set; }
        public required string City { get; set; }
    }

    public enum BCeIDOptionSet
    {
        Yes = 172580000,
        No = 172580001
    }
}
