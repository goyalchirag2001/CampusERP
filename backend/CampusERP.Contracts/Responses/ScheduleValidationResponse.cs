namespace CampusERP.Contracts.Responses;

public class ScheduleValidationResponse
{
    public bool IsValid { get; set; }

    public bool HasConflicts => Conflicts.Count > 0;

    public bool CanAutoOverride { get; set; }

    public List<ScheduleConflictResponse> Conflicts { get; set; } = [];

    public List<TimetableTemplateResponse> AffectedLectures { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public List<string> Information { get; set; } = [];
}