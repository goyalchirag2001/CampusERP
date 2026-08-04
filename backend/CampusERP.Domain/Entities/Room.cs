using System.ComponentModel.DataAnnotations;
using CampusERP.Domain.Common;
using CampusERP.Shared.Enums;

namespace CampusERP.Domain.Entities;

public class Room : BaseEntity, ITenantEntity
{
    #region Foreign Keys

    public Guid InstitutionId { get; set; }

    public Guid CampusId { get; set; }

    #endregion

    #region Basic Information

    [Required]
    [MaxLength(100)]
    public string Building { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Floor { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoomNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string RoomName { get; set; } = string.Empty;

    [Required]
    public RoomType RoomType { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? LocationCode { get; set; }

    #endregion

    #region Capacity

    public int Capacity { get; set; }

    public int DisplayOrder { get; set; }

    #endregion

    #region Facilities

    public bool HasProjector { get; set; }

    public bool HasSmartBoard { get; set; }

    public bool HasAirConditioning { get; set; }

    public bool HasComputers { get; set; }

    public bool HasInternet { get; set; }

    public bool IsAccessible { get; set; }

    #endregion

    #region Status

    public bool IsActive { get; set; } = true;

    #endregion

    #region Navigation Properties

    public Institution Institution { get; set; } = null!;

    public Campus Campus { get; set; } = null!;

    public ICollection<TimetableTemplate> TimetableTemplates { get; set; } = new List<TimetableTemplate>();

    public ICollection<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();

    public ICollection<LectureOverride> OriginalLectureOverrides { get; set; } = new List<LectureOverride>();

    public ICollection<LectureOverride> NewLectureOverrides { get; set; } = new List<LectureOverride>();

    #endregion
}