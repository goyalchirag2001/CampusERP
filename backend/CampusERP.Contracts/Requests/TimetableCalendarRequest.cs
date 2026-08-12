namespace CampusERP.Contracts.Requests;

public class TimetableCalendarRequest
{
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public Guid? AcademicSessionId { get; set; }
}