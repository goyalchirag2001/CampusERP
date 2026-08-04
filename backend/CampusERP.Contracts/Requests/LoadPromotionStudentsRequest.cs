namespace CampusERP.Contracts.Requests;

public class LoadPromotionStudentsRequest
{
    public Guid DepartmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }
}