using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Institution : BaseEntity
{
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
    public string? Website { get; set; }

    [MaxLength(1000)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(100)]
    public string LoginSlug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

    [MaxLength(20)]
    public string? PrimaryColor { get; set; }

    [MaxLength(20)]
    public string? SecondaryColor { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Campus> Campuses { get; set; } = new List<Campus>();

    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<Student> Students { get; set; } = new List<Student>();

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<Department> Departments { get; set; } = new List<Department>();

    public ICollection<Semester> Semesters { get; set; } = new List<Semester>();

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}