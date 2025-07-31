using System.Xml.Serialization;

namespace Ubiminds.Domain.Models.XmlModels;

[XmlRoot("PublishedItem")]
public class ReportPublicationModel
{
    public string Title { get; set; }

    public string Countries { get; set; }

    public DateTime PublishedDate { get; set; }

    public ContactInformation ContactInformation { get; set; }
}

public class ContactInformation
{
    [XmlElement("PersonGroup")]
    public List<PersonGroup> PersonGroups { get; set; } = new();
}

public class PersonGroup
{
    [XmlAttribute("sequence")]
    public int Sequence { get; set; }

    public string Name { get; set; }

    public PersonGroupMember PersonGroupMember { get; set; }
}

public class PersonGroupMember
{
    public Person Person { get; set; }
}

public class Person
{
    public string FamilyName { get; set; }
    public string GivenName { get; set; }
    public string DisplayName { get; set; }
    public string JobTitle { get; set; }
    public ContactInfo ContactInfo { get; set; }
}

public class ContactInfo
{
    public Phone Phone { get; set; }
}

public class Phone
{
    public string Number { get; set; }
}