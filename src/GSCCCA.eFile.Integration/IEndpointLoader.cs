using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Text;

namespace GSCCCA.eFile.Integration
{
    /// <summary>
    /// Defines an endpoint loader interface whose implementation will define how to set web service endpoints for your application.
    /// </summary>
    /// <remarks>
    /// Implement this interface to define your own method for endpoint storage. (databse, user input, local user settings, etc.).
    /// You can then pass your custom IEndpointLoader to the FilerService or CountyService
    /// objects which then create the connection and make calls to the eFile web services.
    /// </remarks>
    /// <example>
    /// This is the implmentation for the ConfigFileEndpointLoader which is included with this library. If you wish to create your own custom loader, you can 
    /// use this as an example in order to do so.
    /// <code>
    ///     public class ConfigFileEndpointLoader : IEndpointLoader
    ///     {
    ///        public string LoadEndpointBase()
    ///        {
    ///            return ConfigurationManager.AppSettings["ENDPOINT"];
    ///        }
    ///        
    ///        public string LoadAccountEndpoint()
    ///        {
    ///            return ConfigurationManager.AppSettings["ACT_ENDPOINT_ASMX"];
    ///        }
    ///
    ///        public string LoadUCCEndpoint()
    ///        {
    ///            return ConfigurationManager.AppSettings["UCC_ENDPOINT_ASMX"];
    ///        }
    ///        
    ///        public BasicHttpBinding LoadBinding()
    ///        {
    ///            return new BasicHttpBinding
    ///                   {
    ///                     Name = "org.gsccca.ucc",
    ///                     MaxReceivedMessageSize = int.MaxValue,
    ///                     MaxBufferSize = int.MaxValue,
    ///                     CloseTimeout = new TimeSpan(0, 5, 0),
    ///                     OpenTimeout = new TimeSpan(0, 5, 0),
    ///                     ReceiveTimeout = new TimeSpan(0, 5, 0),
    ///                     SendTimeout = new TimeSpan(0, 5, 0)
    ///                    };
    ///         }
    ///     }
    /// </code>
    /// </example>
    public interface IEndpointLoader
    {
        /// <summary>
        /// Defines the base address for the website.
        /// </summary>
        /// <returns></returns>
        string LoadWebPageUrl();
        /// <summary>
        /// Defines the address Uri without the ASMX page appended.
        /// </summary>
        /// <returns></returns>
        string LoadEndpointBase();
        /// <summary>
        /// Defines the ASMX page for the UCC API. This will be appended to the base endpoing Uri.
        /// </summary>
        /// <returns></returns>
        string LoadUCCEndpoint();
        /// <summary>
        /// Defines the ASMX page for the Account API. This will be appended to the base endpoing Uri.
        /// </summary>
        /// <returns></returns>
        string LoadAccountEndpoint();
        /// <summary>
        /// Defines a BasicHttpBinding object. This will be used by the service object.
        /// </summary>
        /// <returns></returns>
        System.ServiceModel.BasicHttpBinding LoadBinding();
    }


    /// <summary>
    /// Example endpoint loader. This loader gets the endpoint locations from a configuration file.
    ///  It is also the default loader if no other loader is supplied. 
    /// </summary>
    public class ConfigFileEndpointLoader : IEndpointLoader
    {
        public string LoadWebPageUrl()
        {
            return ConfigurationManager.AppSettings["WEBADDRESS"];
        }

        /// <summary>
        /// Loads from your AppSettings key="ENDPOINT"
        /// </summary>
        /// <returns></returns>
        public string LoadEndpointBase()
        {
            return ConfigurationManager.AppSettings["ENDPOINT"];
        }

        /// <summary>
        /// Loads from your AppSettings key="ACT_ENDPOINT_ASMX"
        /// </summary>
        /// <returns></returns>
        public string LoadAccountEndpoint()
        {
            return ConfigurationManager.AppSettings["ACT_ENDPOINT_ASMX"];
        }

        /// <summary>
        /// Loads from your AppSettings key="UCC_ENDPOINT_ASMX"
        /// </summary>
        /// <returns></returns>
        public string LoadUCCEndpoint()
        {
            return ConfigurationManager.AppSettings["UCC_ENDPOINT_ASMX"];
        }

        /// <summary>
        /// Loads a hard coded binding, defined at compile time.
        /// </summary>
        /// <returns></returns>
        public BasicHttpBinding LoadBinding()
        {
            return new BasicHttpBinding
            {
                Name = "org.gsccca.ucc",
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                CloseTimeout = new TimeSpan(0, 5, 0),
                OpenTimeout = new TimeSpan(0, 5, 0),
                ReceiveTimeout = new TimeSpan(0, 5, 0),
                SendTimeout = new TimeSpan(0, 5, 0)
            };
        }
    }
}
