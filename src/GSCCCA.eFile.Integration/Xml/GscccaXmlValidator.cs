using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace GSCCCA.eFile.Integration.Xml
{
    /// <summary>
    /// The GSCCCA Validator will validate XML against a given schema.
    /// This class assumes that the XML Schema is stored on disk and that the XML is
    ///  stored in a string.
    /// If you wish to use your own validator, you can implement the IXmlValidator 
    ///  interface that is also located in this namespace and use your own.
    /// </summary>
    public class GscccaXmlValidator : IXmlValidator
    {
        /// <summary>
        /// </summary>
        /// <param name="schemaUri">The location of the XSD that the validator will use</param>
        public GscccaXmlValidator(string schemaUri)
        {
            Inputs<string>.CheckNull(schemaUri, nameof(schemaUri));
            Inputs<string>.CheckEmpty(schemaUri, nameof(schemaUri));

            SchemaUri = schemaUri;
            SchemaNamespace = string.Empty;
        }
        /// <summary>
        /// </summary>
        /// <param name="schemaUri">The location of the XSD that the validator will use</param>
        /// <param name="schemaNamespace">The namespace that the validator will use</param>
        public GscccaXmlValidator(string schemaUri, string schemaNamespace)
        {
            Inputs<string>.CheckNull(schemaUri, nameof(schemaUri));
            Inputs<string>.CheckEmpty(schemaUri, nameof(schemaUri));
            Inputs<string>.CheckNull(schemaNamespace, nameof(schemaNamespace));
            Inputs<string>.CheckEmpty(schemaNamespace, nameof(schemaNamespace));

            SchemaUri = schemaUri;
            SchemaNamespace = schemaNamespace;
        }

        /// <summary>
        /// Validates given XML against the schema
        /// </summary>
        /// <param name="xml">A string containing the XML data to validate</param>
        /// <param name="validatorCallback">Will be called when a schema violation is found. The default if no callback is supplied is to throw an ArgumentException.</param>
        public void Validate(string xml, Action<object, ValidationEventArgs> validatorCallback = null)
        {
            Inputs<string>.CheckNull(xml, nameof(xml));
            Inputs<string>.CheckEmpty(xml, nameof(xml));

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(SchemaNamespace, SchemaUri);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(schemaSet);
            settings.ValidationEventHandler += new ValidationEventHandler(validatorCallback ?? DefaultValidatorCallback);
            settings.ValidationType = ValidationType.Schema;

            using (MemoryStream xmlStream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                using (XmlReader reader = XmlReader.Create(xmlStream, settings))
                {
                    while (reader.Read()) { }
                }
            }
        }

        private void DefaultValidatorCallback(object sender, ValidationEventArgs args)
        {
            throw new ArgumentException($"The parameter 'xml' is not valid as per the schema. {args.Severity}:{args.Message}");
        }

        /// <summary>
        /// The location on disk taht the schema can be found
        /// </summary>
        public string SchemaUri { get; private set; }
        /// <summary>
        /// The namespace that will used to validate
        /// </summary>
        public string SchemaNamespace { get; private set; }
    }
}
