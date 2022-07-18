using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Filers
{
    public class CallDepositDrawdownFunds
    {
        public static async Task<bool> ServiceAsync(string user, string password, string token, decimal amount, string endpoint)
        {
            using (FilerService service = new FilerService(endpoint))
            {
                return await service.CallDepositEscrowAsync(user, password, token, amount);
            }
        }
    }
}
