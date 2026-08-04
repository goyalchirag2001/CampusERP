using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;
using CampusERP.Shared.Enums;

namespace CampusERP.Application.Mappings;

public static class AttendanceCorrectionRequestMapper
{
    public static AttendanceCorrectionRequestResponse ToResponse(AttendanceCorrectionRequest entity)
    {
        return new AttendanceCorrectionRequestResponse
        {
            #region Basic

            Id = entity.Id,

            AttendanceRecordId = entity.AttendanceRecordId,

            StudentId = entity.StudentId,

            #endregion

            #region Request

            Reason = entity.Reason,

            ReasonName = entity.Reason.ToString(),

            OriginalStatus = entity.OriginalStatus,

            OriginalStatusName = entity.OriginalStatus.ToString(),

            RequestedStatus = entity.RequestedStatus,

            RequestedStatusName = entity.RequestedStatus.ToString(),

            Status = entity.Status,

            StatusName = entity.Status.ToString(),

            Description = entity.Description,

            AttachmentPath = entity.AttachmentPath,

            #endregion

            #region Review

            ReviewedByUserId = entity.ReviewedByUserId,

            ReviewedByName = entity.ReviewedByUser == null? null: $"{entity.ReviewedByUser.FirstName} {entity.ReviewedByUser.LastName}".Trim(),
            
            ReviewedOn = entity.ReviewedOn,

            ReviewRemarks = entity.ReviewRemarks,

            #endregion

            #region Processing

            IsProcessed = entity.IsProcessed,

            ProcessedOn = entity.ProcessedOn,

            AttendanceUpdatedOn = entity.AttendanceUpdatedOn,

            #endregion

            #region Audit

            CreatedAt = entity.CreatedAt,

            CreatedBy = entity.CreatedBy

            #endregion
        };
    }

    public static List<AttendanceCorrectionRequestResponse> ToResponse(IEnumerable<AttendanceCorrectionRequest> entities)
    {
        return entities
            .Select(ToResponse)
            .ToList();
    }

}