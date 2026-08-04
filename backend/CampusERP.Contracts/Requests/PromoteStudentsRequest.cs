namespace CampusERP.Contracts.Requests;

public class PromoteStudentsRequest
{
    public List<PromoteStudentItem> Students { get; set; } = [];
}

public class PromoteStudentItem
{
    public Guid StudentId { get; set; }

    public Guid CurrentEnrollmentId { get; set; }

    public Guid NextSectionId { get; set; }
}