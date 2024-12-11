using AutoMapper;
using EMCR.DRR.Dynamics;
using Microsoft.Dynamics.CRM;
using System.Threading.Tasks;

namespace EMCR.DRR.API.Resources.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ILogger<AccountRepository> logger;
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;

        public AccountRepository(IDRRContextFactory dRRContextFactory, IMapper mapper, ILogger<AccountRepository> logger)
        {
            this.dRRContextFactory = dRRContextFactory;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<ManageAccountCommandResult> Manage(ManageAccountCommand cmd)
        {
            return cmd switch
            {
                SaveAccountIfNotExists c => await HandleSaveAccountIfNotExists(c),
                _ => throw new NotSupportedException($"{cmd.GetType().Name} is not supported")
            };
        }

        public async Task<ManageAccountCommandResult> HandleSaveAccountIfNotExists(SaveAccountIfNotExists cmd)
        {
            var ctx = dRRContextFactory.Create();
            var account = mapper.Map<account>(cmd.Account);
            var existingAccounts = await ctx.accounts.Where(a => a.drr_bceidguid == account.drr_bceidguid).ToListAsync();
            if (existingAccounts == null || !existingAccounts.Any())
            {
                account.accountid = Guid.NewGuid();
                ctx.AddToaccounts(account);
                await ctx.SaveChangesAsync();
            }
            else
            {
                if (existingAccounts.Count > 1)
                {
                    logger.LogWarning($"More than one account exists with the BCeID: {account.drr_bceidguid}");
                }

                if (existingAccounts.Count == 1)
                {
                    var existingAccount = existingAccounts.First();
                    if (string.IsNullOrEmpty(existingAccount.address1_city))
                    {
                        existingAccount.address1_city = cmd.Account.City;
                        ctx.UpdateObject(existingAccount);
                        await ctx.SaveChangesAsync();
                    }
                }

                account.accountid = existingAccounts.First().accountid;
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            return new ManageAccountCommandResult { Id = account.accountid.ToString() };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
