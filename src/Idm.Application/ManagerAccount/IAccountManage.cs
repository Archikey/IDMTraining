using Idm.Domain.Entities;

namespace Idm.Application.ManagerAccount;

    public interface IAccountManage
    {
        public Task<Account?> CreateAccountAsync(Account account);

        public Task<Account?> FindAccountByIdAsync(int id);

        public Task<List<Account>> FindAccountByBelongingToSystemAsync(int systemId);

        public Task<List<AccountStatistics>> GetAccountStatisticsAsync();


    }
