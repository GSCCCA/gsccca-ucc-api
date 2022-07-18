using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GSCCCA.eFile.Integration.Xml
{
    /// <summary>
    /// XmlFormatter is used to convert string XML into a .NET object T and vice versa.
    /// </summary>
    /// <typeparam name="T">An IXmlSeriializable .NET object</typeparam>
    /// <remarks>
    /// <example>
    /// In order to deserialize perform the following call.
    /// <code>
    /// var FilingData = XmlFormatter{GSCCCA.eFile.Integration.IacaFile.Document}.Deserialize(xml);
    /// </code>
    /// In order to serialize perform the followign call.
    /// <code>
    /// var xml = XmlFormatter{GSCCCA.eFile.Integration.IacaFile.Document}.Serialize(FilingData);
    /// </code>
    /// </example>
    /// </remarks>
    public class XmlFormatter<T> where T : IXmlSerializable
    {
        /// <summary>
        /// Deserializes XML representing a T object into that object
        /// </summary>
        /// <param name="container">Object that contains the XML keyed to a given T</param>
        public static T Deserialize(GscccaXmlContainer container)
        {
            // Deserialize
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (System.IO.StringReader reader = new System.IO.StringReader(container.XML.ToString()))
            {
                T response = (T)serializer.Deserialize(reader);
                return response;
            }
        }

        /// <summary>
        /// Deserializes XML representing a T object into that object
        /// </summary>
        /// <param name="xml">XML string of a given T</param>
        public static T Deserialize(string xml)
        {
            // Deserialize
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (System.IO.StringReader reader = new System.IO.StringReader(xml))
            {
                T response = (T)serializer.Deserialize(reader);
                return response;
            }
        }

        /// <summary>
        /// Serilize a T into string xml
        /// </summary>
        /// <param name="xmlObject">T object to serialize</param>
        /// <returns>xml string</returns>
        public static string Serialize(T xmlObject)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = true,
                NewLineChars = string.Empty,
                NewLineHandling = NewLineHandling.None
            };

            using (StringWriter writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    serializer.Serialize(xmlWriter, xmlObject, namespaces);

                    return writer.ToString();
                }
            }
        }
    }
}