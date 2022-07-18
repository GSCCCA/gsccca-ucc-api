using GSCCCA.eFile.Integration.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GSCCCA.eFile.Integration
{
    /// <summary>
    /// Service class that makes calls to the Clerks Authority UCC and Account APIs
    /// For use by counties who are processing submittted UCCs
    /// </summary>
    /// <remarks>
    /// This should be called in a using statement in order to dispose of the connection when completed.
    /// <code>
    ///         using (var service = new CountyService(loader))
    ///         {
    ///             return await service.CallCheckCredentialsAsync(user, password);
    ///         }
    /// </code>
    /// </remarks>
    public class CountyService : IDisposable
    {
        /// <summary>
        /// When used, will use the default IEndpointLoader to get all endpoint values.
        /// </summary>
        public CountyService() : this("") { }

        /// <summary>
        /// When used, will use the default IEndpointLoader to get the ASMX pages, but the base endpoint is set by the <paramref name="endpointBase"/> parameter.
        /// </summary>
        /// <param name="endpointBase">Defines the address Uri without the ASMX page</param>
        public CountyService(string endpointBase) : this(endpointBase, new ConfigFileEndpointLoader()) { }

        /// <summary>
        /// When used, will use the custom given IEndpoingLoader to get all endpoint values.
        /// </summary>
        /// <param name="loader">Defines how endpoints will be set</param>
        public CountyService(IEndpointLoader loader) : this(string.Empty, loader) { }

        /// <summary>
        /// When used, will load the endpoint addresses using the loader, but it will set the endpoint base given
        /// </summary>
        /// <param name="endpointBase">Defines the address Uri without the ASMX page</param>
        /// <param name="loader">Defines how endpoints will be set</param>
        public CountyService(string endpointBase, IEndpointLoader loader)
        {
            if (string.IsNullOrEmpty(endpointBase))
                endpointBase = loader.LoadEndpointBase();

            Uri endpointUri = new Uri(endpointBase);
            EndpointAddress actEndpoint = new EndpointAddress(endpointBase + loader.LoadAccountEndpoint());
            EndpointAddress uccEndpoint = new EndpointAddress(endpointBase + loader.LoadUCCEndpoint());
            BasicHttpBinding binding = loader.LoadBinding();

            if (endpointUri.Scheme.ToLower() == "https")
                binding.Security.Mode = BasicHttpSecurityMode.Transport;

            AccountService = new org.gsccca.efile.accountservice.AccountServiceSoapClient(binding, actEndpoint);
            UCCService = new org.gsccca.efile.uccservice.UCCServiceSoapClient(binding, uccEndpoint);
        }

        /// <summary>
        /// Calls Account.CheckCredentials()
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <returns>true if credentials are valid</returns>
        public async Task<bool> CallCheckCredentialsAsync(string user, string password)
        {
            org.gsccca.efile.accountservice.CheckCredentialsResponse response = await AccountService.CheckCredentialsAsync(user, password);
            return response.Body.CheckCredentialsResult;
        }

        /// <summary>
        /// Calls UCC.GetQueue
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <returns>array of filing identifiers</returns>
        public async Task<string[]> CallGetQueueAsync(string user, string password)
        {
            var response =  await UCCService.GetQueueAsync(user, password);
            return response.Body.GetQueueResult.ToArray();
        }

        /// <summary>
        /// Gets the full and most up to date XML data associated with the submitted UCC
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to return</param>
        /// <returns>IACA File XML with Georgia modifications</returns>
        public async Task<IacaFile.v3.FileDocument> CallGetFilingDataAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.GetFilingDataResponse response = await UCCService.GetFilingDataAsync(user, password, id);
            return XmlFormatter<IacaFile.v3.FileDocument>.Deserialize(response.Body.GetFilingDataResult);
        }

        /// <summary>
        /// Gets the PDF document of the given UCC
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to return</param>
        /// <returns>PDF document of filing (stamped if accepted)</returns>
        public async Task<byte[]> CallGetFilingDocumentAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.GetFilingDocumentResponse response = await UCCService.GetFilingDocumentAsync(user, password, id);
            return response.Body.GetFilingDocumentResult;
        }

        /// <summary>
        /// Calls UCCService.Accept()
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to accept</param>
        /// <param name="filenumber">optional parameter used if you are assigning file numbers</param>
        /// <param name="acceptedDateTime">optional parameter used if the accept time needs to be backdated</param>
        /// <returns>IACA File XML object</returns>
        public async Task<IacaFile.v3.FileDocument> CallAcceptAsync(string user, string password, int id, string filenumber = null, DateTime acceptedDateTime = default)
        {
            IacaFile.v3.FileDocument iacaFile;

            if (acceptedDateTime == default)
            {
                var response = await UCCService.AcceptAsync(user, password, id, filenumber);
                iacaFile = XmlFormatter<IacaFile.v3.FileDocument>.Deserialize(response.Body.AcceptResult);
            }
            else
            {
                var response = await UCCService.AcceptSetDateAsync(user, password, id, filenumber, acceptedDateTime.ToString("o"));
                iacaFile = XmlFormatter<IacaFile.v3.FileDocument>.Deserialize(response.Body.AcceptSetDateResult);
            }

            return iacaFile;
        }

        /// <summary>
        /// Calls UCCService.GetRejectionReasons()
        /// </summary>
        /// <returns>An array that contains all valid rejection reasons</returns>
        public async Task<string[]> CallGetRejectionReasonsAsync()
        {
            var response = await UCCService.GetRejectionReasonsAsync();
            return response.Body.GetRejectionReasonsResult.ToArray();
        }

        /// <summary>
        /// Calls UCCService.Reject()
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to reject</param>
        /// <param name="reasons">applicable rejection reasons</param>
        /// <returns>true if the UCC is rejected</returns>
        public async Task<bool> CallRejectAsync(string user, string password, int id, params string[] reasons)
        {
            org.gsccca.efile.uccservice.ArrayOfString reasonsConv = new org.gsccca.efile.uccservice.ArrayOfString();
            foreach (var reason in reasons)
                reasonsConv.Add(reason);

            var response = await UCCService.RejectAsync(user, password, id, reasonsConv);
            return response.Body.RejectResult;
        }

        /// <summary>
        /// Calls UCCService.Unreject()
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to reject</param>
        /// <returns>true if the UCC is unrejected</returns>
        public async Task<bool> CallUnrejectAsync(string user, string password, int id)
        {
            var response = await UCCService.UnrejectAsync(user, password, id);
            return response.Body.UnrejectResult;
        }

        /// <summary>
        /// Archives a UCC on the https://efile.gsccca.org dashboard
        /// </summary>
        /// <param name="user">efile.gsccca.org username</param>
        /// <param name="password">efile.gsccca.org password</param>
        /// <param name="id">filing identifier of UCC to archive</param>
        /// <returns>true if the filing was archived</returns>
        public async Task<bool> CallArchiveFilingAsync(string user, string password, int id)
        {
            org.gsccca.efile.uccservice.ArchiveUCCResponse response = await UCCService.ArchiveUCCAsync(user, password, id);
            return response.Body.ArchiveUCCResult;
        }

        private org.gsccca.efile.uccservice.UCCServiceSoapClient UCCService { get; set; }
        private org.gsccca.efile.accountservice.AccountServiceSoapClient AccountService { get; set; }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    (UCCService as IDisposable).Dispose();
                    (AccountService as IDisposable).Dispose();
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    UCCService.Abort();
                    AccountService.Abort();
                    disposed = true;
                }
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
