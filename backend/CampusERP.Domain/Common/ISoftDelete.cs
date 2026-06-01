namespace CampusERP.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}