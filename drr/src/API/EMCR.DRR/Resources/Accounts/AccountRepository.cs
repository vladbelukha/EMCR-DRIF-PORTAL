using AutoMapper;
using EMCR.DRR.Dynamics;
using Microsoft.Dynamics.CRM;

namespace EMCR.DRR.API.Resources.Accounts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDRRContextFactory dRRContextFactory;
        private readonly IMapper mapper;

        public AccountRepository(IDRRContextFactory dRRContextFactory, IMapper mapper)
        {
            this.dRRContextFactory = dRRContextFactory;
            this.mapper = mapper;
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
            var existingAccount = await ctx.accounts.Where(a => a.drr_bceidguid == account.drr_bceidguid).SingleOrDefaultAsync();
            if (existingAccount == null)
            {
                account.accountid = Guid.NewGuid();
                ctx.AddToaccounts(account);
                await ctx.SaveChangesAsync();
            }
            else
            {
                account.accountid = existingAccount.accountid;
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            return new ManageAccountCommandResult { Id = account.accountid.ToString() };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
