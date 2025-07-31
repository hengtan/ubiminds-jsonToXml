namespace Ubiminds.Infrastructure.Xml.Interfaces;

public interface IXmlConverter
{
    string Serialize<T>(T input);
}