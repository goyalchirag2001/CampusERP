using System.ComponentModel.DataAnnotations;

namespace CampusERP.Contracts.Requests;

public class UpdateProfileRequest
{
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
}