using GSCCCA.eFile.Integration.AuthorityWallet;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Filers
{
    public class CallQueryFunds
    {
        public static async Task<Wallet> ServiceAsync(string user, string password, string endpoint)
        {
            using (FilerService service = new FilerService(endpoint))
            {
                return await service.CallQueryFundsAsync(user, password);
            }
        }
    }
}
