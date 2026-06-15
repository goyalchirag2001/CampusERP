using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class SemesterSubject : BaseEntity
{
    public Guid SemesterId { get; set; }

    public Guid SubjectId { get; set; }

    public Semester Semester { get; set; } = null!;

    public Subject Subject { get; set; } = null!;

    public ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
}