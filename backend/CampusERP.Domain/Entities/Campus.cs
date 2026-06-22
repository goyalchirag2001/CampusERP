using CampusERP.Domain.Common;
using CampusERP.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Campus : BaseEntity, ITenantEntity
{
    public Guid InstitutionId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(200)]
    public string? CampusHeadName { get; set; }

    public bool IsActive { get; set; } = true;

    public Institution Institution { get; set; } = null!;

    public ICollection<Department> Departments { get; set; } = new List<Department>();

    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<Student> Students { get; set; } = new List<Student>();

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<Semester> Semesters { get; set; } = new List<Semester>();

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}