using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Ubiminds.Domain.Models.InputModels;
using Ubiminds.Infrastructure.Interfaces;

namespace Ubiminds.Infrastructure.Services;

public class XmlConverterService : IXmlConverterService
{
    public string ConvertToXml(DocumentInputModel model)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));

        var root = new XElement("PublishedItem");

        AppendAdditionalData(root, model.AdditionalData);
        AppendStandardFields(root, model);

        var xmlDoc = new XDocument(
            new XDeclaration("1.0", "utf-16", null),
            root
        );

        return SerializeXml(xmlDoc);
    }

    private static void AppendAdditionalData(XElement root, Dictionary<string, JsonElement>? additionalData)
    {
        if (additionalData is null) return;

        foreach (var (key, value) in additionalData)
        {
            var element = ConvertJsonElementToXml(key, value);
            root.Add(element);
        }
    }

    private static void AppendStandardFields(XElement root, DocumentInputModel model)
    {
        root.Add(
            new XElement("Title", model.Title),
            new XElement("PublishDate", model.PublishDate.ToString("o")),
            new XElement("Status", model.Status),
            new XElement("TestRun", model.TestRun)
        );
    }

    private static string SerializeXml(XDocument document)
    {
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false
        });

        document.Save(xmlWriter);
        xmlWriter.Flush();

        return stringWriter.ToString();
    }

    private static XElement ConvertJsonElementToXml(string key, JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => new XElement(key,
                element.EnumerateObject()
                    .Select(prop => ConvertJsonElementToXml(prop.Name, prop.Value))),

            JsonValueKind.Array => new XElement(key,
                element.EnumerateArray()
                    .Select(item => ConvertJsonElementToXml("Item", item))),

            JsonValueKind.String => new XElement(key, element.GetString()),
            JsonValueKind.Number => new XElement(key, element.GetRawText()),
            JsonValueKind.True or JsonValueKind.False => new XElement(key, element.GetBoolean()),
            _ => new XElement(key, string.Empty)
        };
    }
}