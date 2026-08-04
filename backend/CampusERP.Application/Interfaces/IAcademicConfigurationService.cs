using CampusERP.Contracts.Requests;
using CampusERP.Contracts.Responses;
using CampusERP.Domain.Entities;

namespace CampusERP.Application.Interfaces;

public interface IAcademicConfigurationService
{
    Task<AcademicConfigurationResponse> GetAsync();

    Task<AcademicConfigurationResponse> UpdateAsync(UpdateAcademicConfigurationRequest request);

    Task<AcademicConfiguration> GetEffectiveConfigurationAsync(Guid institutionId, Guid? campusId);
}