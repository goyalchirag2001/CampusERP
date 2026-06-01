using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Student : BaseEntity
{
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string RollNumber { get; set; } = string.Empty;

    public Guid CourseId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Batch { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public User User { get; set; } = null!;

    public Course Course { get; set; } = null!;
}