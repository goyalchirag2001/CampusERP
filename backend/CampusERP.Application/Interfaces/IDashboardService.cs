using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetPlatformDashboardAsync();
}