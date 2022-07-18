using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration.UCC.Demo.Examples.Counties
{
    public class CallAccept
    {
        public static async Task<IacaFile.v3.FileDocument> ServiceAsync(string user, 
                                                                 string password, 
                                                                 int filingid, 
                                                                 string endpoint, 
                                                                 string filenumber = null, 
                                                                 DateTime backdate = default)
        {
            using (CountyService service = new CountyService(endpoint))
            {
                return await service.CallAcceptAsync(user: user,
                                                     password: password,
                                                     id: filingid,
                                                     filenumber: filenumber,
                                                     acceptedDateTime: backdate);
            }
        }
    }
}
