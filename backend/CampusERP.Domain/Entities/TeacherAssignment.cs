using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class TeacherAssignment : BaseEntity
{
    public Guid TeacherId { get; set; }

    public Guid SemesterSubjectId { get; set; }

    public Guid SectionId { get; set; }

    public Guid AcademicSessionId { get; set; }

    #region Navigation Properties

    public Teacher Teacher { get; set; } = null!;

    public SemesterSubject SemesterSubject { get; set; } = null!;

    public Section Section { get; set; } = null!;

    public AcademicSession AcademicSession { get; set; } = null!;

    public ICollection<TimetableTemplate> TimetableTemplates { get; set; } = new List<TimetableTemplate>();

    #endregion
}