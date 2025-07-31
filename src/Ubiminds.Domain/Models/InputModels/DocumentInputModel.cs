namespace Ubiminds.Domain.Models.InputModels;
public class DocumentInputModel
{
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public int Status { get; set; }
    public bool TestRun { get; set; }

    public bool IsValidForXml()
    {
        return Status == 3
               && PublishDate >= new DateTime(2024, 08, 24)
               && TestRun;
    }
}
