using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;

namespace CampusERP.Application.Interfaces;

public interface IStudentPromotionService
{
    Task<List<PromotionStudentResponse>> LoadStudentsAsync(LoadPromotionStudentsRequest request);

    Task PromoteAsync(PromoteStudentsRequest request);
}