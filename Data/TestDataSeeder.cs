using System;
using System.Collections.Generic;
using OrgnTransplant.Models;
using OrgnTransplant.Utilities;

namespace OrgnTransplant.Data
{
    /// <summary>
    /// Клас за зареждане на тестови данни в базата данни
    /// </summary>
    public class TestDataSeeder
    {
        /// <summary>
        /// Зарежда тестови донори в базата данни
        /// ВАЖНО: Базата данни трябва да е празна преди да извикате този метод
        /// </summary>
        public static void SeedTestData()
        {
            try
            {
                // Проверка дали базата данни е празна
                var existingDonors = DatabaseHelper.GetAllDonors();
                if (existingDonors.Count > 0)
                {
                    throw new Exception("Базата данни вече съдържа записи. Изтрийте ги първо с: DELETE FROM Donors;");
                }

                DateTime now = DateTime.Now;

                List<Donor> testDonors = new List<Donor>
                {
                    // ========== ПОЧИНАЛИ ДОНОРИ (DECEASED) ==========

                    // Починал донор 1 - София (Спешна трансплантация - 2 часа)
                    new Donor {
                        FullName = "Иван Петров Георгиев",
                        Hospital = "УМБАЛ Александровска - София",
                        DateOfBirth = new DateTime(1985, 3, 15),
                        Gender = "Мъж",
                        NationalId = "8503154321",
                        Phone = "0888123456",
                        Email = "",
                        Address = "ул. Витоша 15, София",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Сърце, Бял дроб",
                        OrganHarvestTime = now.AddHours(-2),
                        OrganQuality = "Excellent",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddDays(-1),
                        CauseOfDeath = "Мозъчна травма от катастрофа",
                        FamilyConsentGivenBy = "Мария Петрова",
                        FamilyRelationship = "Съпруга",
                        FamilyContactPhone = "0888654321",
                        AdditionalNotes = "Спешна трансплантация - сърцето трябва да бъде трансплантирано до 4 часа",
                        RegisteredBy = "Д-р Иванов",
                        CreatedAt = now.AddHours(-2)
                    },

                    // Починал донор 2 - Пловдив (Бъбрек + Черен дроб)
                    new Donor {
                        FullName = "Георги Димитров Стоянов",
                        Hospital = "УМБАЛ Св. Георги - Пловдив",
                        DateOfBirth = new DateTime(1978, 7, 22),
                        Gender = "Мъж",
                        NationalId = "7807225678",
                        Phone = "0899234567",
                        Email = "",
                        Address = "бул. Марица 45, Пловдив",
                        BloodType = "O",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Бъбрек, Черен дроб",
                        OrganHarvestTime = now.AddHours(-6),
                        OrganQuality = "Good",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddDays(-1),
                        CauseOfDeath = "Мозъчна смърт след инсулт",
                        FamilyConsentGivenBy = "Елена Стоянова",
                        FamilyRelationship = "Сестра",
                        FamilyContactPhone = "0899876543",
                        AdditionalNotes = "Бъбреците са в отлично състояние",
                        RegisteredBy = "Д-р Петрова",
                        CreatedAt = now.AddHours(-6)
                    },

                    // Живи донори
                    new Donor {
                        FullName = "Мария Иванова Николова",
                        Hospital = "УМБАЛ Александровска - София",
                        DateOfBirth = new DateTime(1990, 5, 12),
                        Gender = "Жена",
                        NationalId = "9005121234",
                        Phone = "0877123456",
                        Email = "maria.nikolova@email.bg",
                        Address = "ул. Цар Борис 89, София",
                        BloodType = "B",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Бъбрек",
                        OrganHarvestTime = now,
                        OrganQuality = "Excellent",
                        DonorType = "Living",
                        AdditionalNotes = "Жив донор - даряване на бъбрек на близък роднина",
                        RegisteredBy = "Д-р Георгиев",
                        CreatedAt = now
                    },

                    // Добавете още тестови донори тук...
                };

                // Запис на всички тестови донори
                foreach (var donor in testDonors)
                {
                    DatabaseHelper.SaveDonor(donor);
                }

                Logger.LogInfo($"Successfully seeded {testDonors.Count} test donors");
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to seed test data", ex);
                throw;
            }
        }
    }
}
