using System.Xml;
using System.Xml.Serialization;
using Ubiminds.Infrastructure.Xml.Interfaces;

namespace Ubiminds.Infrastructure;

public class XmlConverter : IXmlConverter
{
    public string Serialize<T>(T input)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        using var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false
        });
        xmlSerializer.Serialize(writer, input);
        return stringWriter.ToString();
    }
}