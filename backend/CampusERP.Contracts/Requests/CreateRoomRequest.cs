namespace CampusERP.Contracts.Requests;

public class CreateRoomRequest
{
    public Guid CampusId { get; set; }

    public string Building { get; set; } = string.Empty;

    public string Floor { get; set; } = string.Empty;

    public string RoomNumber { get; set; } = string.Empty;

    public string RoomName { get; set; } = string.Empty;

    public string RoomType { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool HasProjector { get; set; }

    public bool HasSmartBoard { get; set; }

    public bool HasAirConditioning { get; set; }

    public bool HasComputers { get; set; }

    public bool HasInternet { get; set; }

    public string? Description { get; set; }

    public string? LocationCode { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsAccessible { get; set; }
}