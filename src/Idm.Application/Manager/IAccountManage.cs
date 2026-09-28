using Idm.Domain.Entities;

namespace Idm.Application.Manager;

    public interface IAccountManage
    {
        public Task<Account?> CreateAccountAsync(Account account);

        public Task<Account?> FindAccountByIdAsync(int id);

        public Task<List<Account>> FindAccountByBelongingToSystemAsync(int systemId);

        public Task<List<AccountStatistics>> GetAccountStatisticsAsync();

        public Task<Account?> UpdateAccountAsync(Account account);

        public Task<bool> DeleteAccountAsync(int id);


    }
