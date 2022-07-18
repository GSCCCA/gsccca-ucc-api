using System;
using System.Xml.Schema;

namespace GSCCCA.eFile.Integration.Xml
{
    /// <summary>
    /// Interface defining an XML Validator used by the Gsccca.eFile.Integration namespace
    /// You can implement this interface to define your own validator.
    /// </summary>
    public interface IXmlValidator
    {
        /// <summary>
        /// Validates given XML against the schema
        /// </summary>
        /// <param name="xml">A string containing the XML data to validate</param>
        /// <param name="validatorCallback">Will be called when a schema violation is found.</param>
        void Validate(string xml, Action<object, ValidationEventArgs> validatorCallback = null);
    }
}
