using System.Collections.Generic;
using OrgnTransplant.Models;
using System.Threading.Tasks;

namespace OrgnTransplant.Data
{
    public interface IDonorService
    {
        Task<List<Donor>> GetAllDonorsAsync();
        Task<List<Donor>> GetDonorsByOrganAsync(string organName);
        Task<Donor?> GetDonorByIdAsync(int donorId);
        Task<bool> SaveDonorAsync(Donor donor);
        Task<bool> UpdateDonorAsync(Donor donor);
        Task<bool> DeleteDonorAsync(int donorId);
        Task<List<OrganInfo>> GetOrganInfoListAsync(string? organName, HospitalLocation? currentHospital, bool showExpired);
    }
}
