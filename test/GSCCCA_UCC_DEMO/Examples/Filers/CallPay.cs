using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Filers
{
    public class CallPay
    {
        public static async Task<bool> ServiceAsync(string user, string password, string token, decimal fee, int id, string endpoint)
        {
            using (FilerService service = new FilerService(endpoint))
            {
                return await service.CallPayAsync(user, password, token, fee, id);
            }
        }
    }
}
