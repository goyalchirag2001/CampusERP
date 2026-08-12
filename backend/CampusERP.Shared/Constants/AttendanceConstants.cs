namespace CampusERP.Shared.Constants;

public static class AttendanceConstants
{
    /// <summary>
    /// Default duration for a QR attendance window.
    /// </summary>
    public const int DefaultQrWindowSeconds = 60;

    /// <summary>
    /// Minimum allowed QR attendance window.
    /// </summary>
    public const int MinimumQrWindowSeconds = 15;

    /// <summary>
    /// Maximum allowed QR attendance window.
    /// </summary>
    public const int MaximumQrWindowSeconds = 300;
}