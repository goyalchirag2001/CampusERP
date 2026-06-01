using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class Course : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10)]
    public int DurationYears { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}