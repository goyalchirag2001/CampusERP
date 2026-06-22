namespace CampusERP.Contracts.Requests;

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}