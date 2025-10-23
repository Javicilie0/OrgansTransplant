using System;
using OrgnTransplant.Utilities;
using OrgnTransplant.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrgnTransplant.Data;

namespace OrgnTransplant.Data
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorRepository;

        public DonorService(IDonorRepository donorRepository)
        {
            _donorRepository = donorRepository;
        }

        public async Task<List<Donor>> GetAllDonorsAsync()
        {
            return await _donorRepository.GetAllDonorsAsync();
        }

        public async Task<List<Donor>> GetDonorsByOrganAsync(string organName)
        {
            return await _donorRepository.GetDonorsByOrganAsync(organName);
        }

        public async Task<Donor?> GetDonorByIdAsync(int donorId)
        {
            return await _donorRepository.GetDonorByIdAsync(donorId);
        }

        public async Task<bool> SaveDonorAsync(Donor donor)
        {
            return await _donorRepository.SaveDonorAsync(donor);
        }

        public async Task<bool> UpdateDonorAsync(Donor donor)
        {
            return await _donorRepository.UpdateDonorAsync(donor);
        }

        public async Task<bool> DeleteDonorAsync(int donorId)
        {
            return await _donorRepository.DeleteDonorAsync(donorId);
        }

        public async Task<List<OrganInfo>> GetOrganInfoListAsync(string? organName, HospitalLocation? currentHospital, bool showExpired)
        {
            List<OrganInfo> organsList = new List<OrganInfo>();

            List<Donor> donors;
            if (string.IsNullOrEmpty(organName))
            {
                donors = await _donorRepository.GetAllDonorsAsync();
            }
            else
            {
                donors = await _donorRepository.GetDonorsByOrganAsync(organName);
            }

            foreach (var donor in donors)
            {
                if (string.IsNullOrEmpty(donor.OrgansForDonation))
                    continue;

                string[] organs = donor.OrgansForDonation.Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string organ in organs)
                {
                    string organTrimmed = organ.Trim();

                    // Filter by specific organ if specified
                    if (!string.IsNullOrEmpty(organName) && organTrimmed != organName)
                        continue;

                    string iconPath = GetOrganIconPath(organTrimmed);
                    double distance = -1;

                    // Calculate viability
                    string viabilityTimeDisplay = OrganViability.FormatRemainingTime(organTrimmed, donor.OrganHarvestTime);
                    string viabilityColor = OrganViability.GetViabilityColor(organTrimmed, donor.OrganHarvestTime);
                    bool isViable = OrganViability.IsOrganViable(organTrimmed, donor.OrganHarvestTime);

                    // Skip expired organs if toggle is off
                    if (!showExpired && !isViable)
                        continue;

                    // Format quality display
                    string qualityDisplay = FormatQualityDisplay(donor.OrganQuality);

                    organsList.Add(new OrganInfo
                    {
                        IconPath = iconPath,
                        OrganName = organTrimmed,
                        Hospital = donor.Hospital ?? "N/A",
                        Donor = donor.FullName ?? "N/A",
                        DistanceKm = distance,
                        AdditionalInfo = $"Кръвна група: {donor.BloodType} {donor.RhFactor}\n" +
                                       $"Телефон: {donor.Phone}\n" +
                                       $"Имейл: {donor.Email}\n" +
                                       $"ЕГН: {donor.NationalId}",
                        ViabilityTimeDisplay = viabilityTimeDisplay,
                        ViabilityColor = viabilityColor,
                        QualityDisplay = qualityDisplay
                    });
                }
            }

            // Load distances asynchronously if hospital is available
            if (currentHospital != null)
            {
                await LoadDistancesAsync(organsList, currentHospital);
                organsList = organsList.OrderBy(o => o.DistanceKm < 0 ? double.MaxValue : o.DistanceKm).ToList();
            }

            return organsList;
        }

        private async Task LoadDistancesAsync(List<OrganInfo> organsList, HospitalLocation currentHospital)
        {
            var tasks = organsList.Select(async organ =>
            {
                if (!string.IsNullOrEmpty(organ.Hospital) && organ.Hospital != "N/A")
                {
                    var donorHospital = HospitalLocation.GetByName(organ.Hospital);
                    if (donorHospital != null)
                    {
                        organ.DistanceKm = await HospitalLocation.GetRoadDistanceAsync(currentHospital, donorHospital);
                    }
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }

        private string GetOrganIconPath(string organName)
        {
            var iconMap = new Dictionary<string, string>
            {
                { "Черва", "Resources/icons/Abdomen/Intestine.png" },
                { "Бъбрек", "Resources/icons/Abdomen/Kidney.png" },
                { "Черен дроб", "Resources/icons/Abdomen/Liver.png" },
                { "Панкреас", "Resources/icons/Abdomen/Pancreas.png" },
                { "Стомах", "Resources/icons/Abdomen/Stomach.png" },
                { "Сърце", "Resources/icons/Chest/Heart.png" },
                { "Бял дроб", "Resources/icons/Chest/lung.png" },
                { "Артерия", "Resources/icons/Chest/Pulmonary Artery.png" },
                { "Тимус", "Resources/icons/Chest/Thymus.png" }
            };

            return iconMap.ContainsKey(organName) ? iconMap[organName] : "Resources/icons/default.png";
        }

        private string FormatQualityDisplay(string? quality)
        {
            if (string.IsNullOrEmpty(quality))
                return "N/A";

            return quality.ToLower() switch
            {
                "excellent" => "⭐ Отлично",
                "good" => "✓ Добро",
                "fair" => "◐ Задоволително",
                "poor" => "✗ Лошо",
                _ => quality
            };
        }
    }
}
