using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.All
{
    public class CallCheckCredentials
    {
        public static async Task<string> ServiceAsync(string user, string password, string endpoint)
        {
            string msg;

            using (FilerService service = new FilerService(endpoint))
            {
                var response = await service.CallCheckCredentialsAsync(user, password);
                if (response)
                    msg = "Connection Success";
                else
                    msg = "Connection Failed";

                return msg;
            }
        }
    }
}