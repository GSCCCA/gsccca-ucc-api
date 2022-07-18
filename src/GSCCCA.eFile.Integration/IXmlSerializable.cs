using System;
using System.Collections.Generic;
using System.Text;

namespace GSCCCA.eFile.Integration
{
    /// <summary>
    /// Used to define allowed XML objects for the <see cref="Xml.XmlFormatter{T}"/> class.
    /// </summary>
    /// <remarks>
    /// <see cref="Xml.XmlFormatter{T}"/> is defined as <code>public class XmlFormatter{T} where T : IXmlSerializable</code>. It can only 
    /// serilaize and deserialze XML which implements this interface. This prevents exceptions from accidently occurring due to setting T incorrectly
    /// and allows for simple future extension of <see cref="Xml.XmlFormatter{T}"/> to handle more XML schemas.
    /// <example>
    /// <code>
    /// var FilingData = XmlFormatter{GSCCCA.eFile.Integration.IacaFile.Document}.Deserialize(FilingXML);
    /// </code>
    /// </example>
    /// </remarks>
    public interface IXmlSerializable
    {
    }
}
