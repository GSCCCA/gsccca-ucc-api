using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace GSCCCA.eFile.Integration.Xml
{
    /// <summary>
    /// The GscccaXmlContainer allows easy access to search and parse xml documents.
    /// </summary>
    public class GscccaXmlContainer
    {
        /// <summary>
        /// </summary>
        /// <param name="xml">XML as an XDocument contianing the entire body</param>
        public GscccaXmlContainer(XDocument xml)
        {
            Inputs<XDocument>.CheckNull(xml, nameof(xml));

            XML = xml;
        }

        /// <summary>
        /// </summary>
        /// <param name="xml">XML as a string contianing the entire body</param>
        public GscccaXmlContainer(string xml)
        {
            Inputs<string>.CheckNull(xml, nameof(xml));
            Inputs<string>.CheckEmpty(xml, nameof(xml));

            StringReader reader = new StringReader(xml);
            XML = XDocument.Load(reader);
        }

        /// <summary>
        /// A parsable XML body
        /// </summary>
        public XDocument XML { get; private set; }

        /// <summary>
        /// Finds all instances of the given element in the XML document and returns it as an IEnumerable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<XElement> this[string name]
        {
            get
            {
                Inputs<string>.CheckNull(name, nameof(name), "GscccaXmlContainer[string]");
                Inputs<string>.CheckEmpty(name, nameof(name), "GscccaXmlContainer[string]");

                if (XML == null)
                    throw new InvalidOperationException("The container has no recorded XML");

                return XML.Descendants(name);
            }
        }
    }
}
