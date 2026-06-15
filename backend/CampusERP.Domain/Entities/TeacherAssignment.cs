using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class TeacherAssignment : BaseEntity
{
    public Guid TeacherId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public SemesterSubject SemesterSubject { get; set; } = null!;
}