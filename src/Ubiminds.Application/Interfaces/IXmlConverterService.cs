using Ubiminds.Domain.Models.InputModels;

namespace Ubiminds.Infrastructure.Interfaces;

public interface IXmlConverterService
{
    string ConvertToXml(DocumentInputModel model);
}