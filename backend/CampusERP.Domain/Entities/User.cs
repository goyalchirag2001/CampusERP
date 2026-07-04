using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;

namespace CampusERP.Domain.Entities;

public class User : BaseEntity, ITenantEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public bool IsActive { get; set; } = true;

    public Student? Student { get; set; }

    public Teacher? Teacher { get; set; }

    [MaxLength(500)]
    public string? ProfilePhotoUrl { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CurrentLoginAt { get; set; }
}