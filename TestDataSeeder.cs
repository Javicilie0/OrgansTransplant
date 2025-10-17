using System;
using System.Collections.Generic;

namespace OrgnTransplant
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
                        DateOfDeath = now.AddHours(-3),
                        CauseOfDeath = "Тежка черепно-мозъчна травма след ПТП",
                        FamilyConsentGivenBy = "Мария Петрова Георгиева",
                        FamilyRelationship = "Съпруг/а",
                        FamilyContactPhone = "0888999111",
                        AdditionalNotes = "Спешна трансплантация - органите са в отлично състояние",
                        RegisteredBy = "Д-р Мария Иванова",
                        CreatedAt = now.AddHours(-2)
                    },

                    // Починал донор 2 - София (5 часа)
                    new Donor {
                        FullName = "Георги Стефанов Николов",
                        Hospital = "Военномедицинска академия - София",
                        DateOfBirth = new DateTime(1978, 11, 8),
                        Gender = "Мъж",
                        NationalId = "7811083456",
                        Phone = "0889111222",
                        Email = "",
                        Address = "ул. Цар Борис 23, София",
                        BloodType = "B",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Черен дроб, Панкреас",
                        OrganHarvestTime = now.AddHours(-5),
                        OrganQuality = "Excellent",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddHours(-6),
                        CauseOfDeath = "Мозъчен инсулт",
                        FamilyConsentGivenBy = "Стефан Георгиев Николов",
                        FamilyRelationship = "Син",
                        FamilyContactPhone = "0889222333",
                        AdditionalNotes = "Семейството дава пълно съгласие за донорство",
                        RegisteredBy = "Д-р Петър Димитров",
                        CreatedAt = now.AddHours(-5)
                    },

                    // Починал донор 3 - Пловдив (20 часа - изтича скоро за Бял дроб)
                    new Donor {
                        FullName = "Димитър Христов Петков",
                        Hospital = "УМБАЛ Св. Георги - Пловдив",
                        DateOfBirth = new DateTime(1988, 5, 12),
                        Gender = "Мъж",
                        NationalId = "8805126789",
                        Phone = "0886222333",
                        Email = "",
                        Address = "ул. Марица 67, Пловдив",
                        BloodType = "A",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Бъбрек, Бял дроб",
                        OrganHarvestTime = now.AddHours(-5).AddMinutes(-30), // 5.5 часа
                        OrganQuality = "Good",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddHours(-6).AddMinutes(-30),
                        CauseOfDeath = "Сърдечна недостатъчност",
                        FamilyConsentGivenBy = "Христина Христова Петкова",
                        FamilyRelationship = "Съпруг/а",
                        FamilyContactPhone = "0886333444",
                        AdditionalNotes = "Бял дроб близо до изтичане на годност (6 часа лимит)",
                        RegisteredBy = "Д-р Иван Стоянов",
                        CreatedAt = now.AddHours(-5).AddMinutes(-30)
                    },

                    // Починал донор 4 - Варна (18 часа - с инфекция "Друго")
                    new Donor {
                        FullName = "Стефан Атанасов Кирилов",
                        Hospital = "УМБАЛ Св. Марина - Варна",
                        DateOfBirth = new DateTime(1983, 12, 25),
                        Gender = "Мъж",
                        NationalId = "8312259012",
                        Phone = "0884444555",
                        Email = "",
                        Address = "ул. Черно море 89, Варна",
                        BloodType = "O",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Друго",
                        InfectiousDiseasesOther = "Леко възпаление на горните дихателни пътища - прекарано преди 2 седмици",
                        OrgansForDonation = "Черен дроб",
                        OrganHarvestTime = now.AddHours(-18),
                        OrganQuality = "Good",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddHours(-19),
                        CauseOfDeath = "Удавяне",
                        FamilyConsentGivenBy = "Атанас Кирилов Атанасов",
                        FamilyRelationship = "Брат",
                        FamilyContactPhone = "0884555666",
                        AdditionalNotes = "Прекарано леко респираторно заболяване - не засяга качеството на черния дроб",
                        RegisteredBy = "Д-р Елена Тодорова",
                        CreatedAt = now.AddHours(-18)
                    },

                    // Починал донор 5 - Бургас (3 часа - много свеж)
                    new Donor {
                        FullName = "Николай Василев Стоянов",
                        Hospital = "УМБАЛ Бургас АД - Бургас",
                        DateOfBirth = new DateTime(1987, 8, 7),
                        Gender = "Мъж",
                        NationalId = "8708072345",
                        Phone = "0882666777",
                        Email = "",
                        Address = "ул. Александровска 34, Бургас",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Сърце, Бял дроб",
                        OrganHarvestTime = now.AddHours(-3),
                        OrganQuality = "Excellent",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddHours(-4),
                        CauseOfDeath = "Инфаркт на миокарда",
                        FamilyConsentGivenBy = "Десислава Василева Стоянова",
                        FamilyRelationship = "Дъщеря",
                        FamilyContactPhone = "0882777888",
                        AdditionalNotes = "Много свежи органи - отлично състояние",
                        RegisteredBy = "Д-р Георги Петров",
                        CreatedAt = now.AddHours(-3)
                    },

                    // Починал донор 6 - Русе (10 часа)
                    new Donor {
                        FullName = "Петър Димитров Стефанов",
                        Hospital = "УМБАЛ Канев - Русе",
                        DateOfBirth = new DateTime(1981, 6, 20),
                        Gender = "Мъж",
                        NationalId = "8106204567",
                        Phone = "0880888999",
                        Email = "",
                        Address = "ул. Дунавска 56, Русе",
                        BloodType = "AB",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Черен дроб, Панкреас",
                        OrganHarvestTime = now.AddHours(-10),
                        OrganQuality = "Good",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddHours(-11),
                        CauseOfDeath = "Мозъчен кръвоизлив",
                        FamilyConsentGivenBy = "Мариана Димитрова Стефанова",
                        FamilyRelationship = "Съпруг/а",
                        FamilyContactPhone = "0880999000",
                        AdditionalNotes = "Средна годност - трябва да се транспланира скоро",
                        RegisteredBy = "Д-р Николай Иванов",
                        CreatedAt = now.AddHours(-10)
                    },

                    // Починал донор 7 - Пловдив (25 часа - изтича за бъбреци)
                    new Donor {
                        FullName = "Васил Тодоров Николов",
                        Hospital = "УМБАЛ Пулмед - Пловдив",
                        DateOfBirth = new DateTime(1986, 1, 28),
                        Gender = "Мъж",
                        NationalId = "8601286789",
                        Phone = "0888222333",
                        Email = "",
                        Address = "ул. Цар Симеон 12, Пловдив",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Бъбрек",
                        OrganHarvestTime = now.AddHours(-25),
                        OrganQuality = "Fair",
                        DonorType = "Deceased",
                        DateOfDeath = now.AddHours(-26),
                        CauseOfDeath = "Белодробна емболия",
                        FamilyConsentGivenBy = "Надежда Атанасова Димитрова",
                        FamilyRelationship = "Родител",
                        FamilyContactPhone = "0888333444",
                        AdditionalNotes = "Бъбрекът е близо до края на годността (36 часа лимит) - качество Fair",
                        RegisteredBy = "Д-р Стефан Георгиев",
                        CreatedAt = now.AddHours(-25)
                    },

                    // ========== ЖИВИ ДОНОРИ (LIVING) ==========

                    // Жив донор 1 - София (регистриран преди 30 дни)
                    new Donor {
                        FullName = "Мария Иванова Димитрова",
                        Hospital = "МБАЛ Св. Анна - София",
                        DateOfBirth = new DateTime(1992, 7, 22),
                        Gender = "Жена",
                        NationalId = "9207225678",
                        Phone = "0887654321",
                        Email = "maria.ivanova@email.com",
                        Address = "бул. България 45, София",
                        BloodType = "O",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Бъбрек",
                        OrganHarvestTime = DateTime.MinValue,
                        OrganQuality = "",
                        DonorType = "Living",
                        DateOfDeath = null,
                        CauseOfDeath = "",
                        FamilyConsentGivenBy = "",
                        FamilyRelationship = "",
                        FamilyContactPhone = "",
                        AdditionalNotes = "Доброволна регистрация - желае да дари бъбрек след смърт",
                        RegisteredBy = "Мед. сестра Елена Петкова",
                        CreatedAt = now.AddDays(-30)
                    },

                    // Жив донор 2 - Пловдив (регистриран преди 15 дни)
                    new Donor {
                        FullName = "Елена Тодорова Василева",
                        Hospital = "УМБАЛ Пулмед - Пловдив",
                        DateOfBirth = new DateTime(1995, 9, 30),
                        Gender = "Жена",
                        NationalId = "9509308901",
                        Phone = "0885333444",
                        Email = "elena.todorova@email.com",
                        Address = "бул. Васил Априлов 12, Пловдив",
                        BloodType = "AB",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Сърце, Бъбрек",
                        OrganHarvestTime = DateTime.MinValue,
                        OrganQuality = "",
                        DonorType = "Living",
                        DateOfDeath = null,
                        CauseOfDeath = "",
                        FamilyConsentGivenBy = "",
                        FamilyRelationship = "",
                        FamilyContactPhone = "",
                        AdditionalNotes = "Млада донорка - регистрирана след посещение на кампания",
                        RegisteredBy = "Мед. сестра Надя Георгиева",
                        CreatedAt = now.AddDays(-15)
                    },

                    // Жив донор 3 - Варна (регистриран преди 7 дни)
                    new Donor {
                        FullName = "Анна Георгиева Петрова",
                        Hospital = "МБАЛ Св. Анна - Варна",
                        DateOfBirth = new DateTime(1990, 4, 18),
                        Gender = "Жена",
                        NationalId = "9004181234",
                        Phone = "0883555666",
                        Email = "anna.georgieva@email.com",
                        Address = "бул. Владислав Варненчик 45, Варна",
                        BloodType = "B",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        InfectiousDiseasesOther = "",
                        OrgansForDonation = "Бъбрек, Панкреас",
                        OrganHarvestTime = DateTime.MinValue,
                        OrganQuality = "",
                        DonorType = "Living",
                        DateOfDeath = null,
                        CauseOfDeath = "",
                        FamilyConsentGivenBy = "",
                        FamilyRelationship = "",
                        FamilyContactPhone = "",
                        AdditionalNotes = "Активист за донорство - мотивирана донорка",
                        RegisteredBy = "Д-р Христо Василев",
                        CreatedAt = now.AddDays(-7)
                    }
                };

                // Запазване на всички тестови донори
                int count = 0;
                foreach (var donor in testDonors)
                {
                    DatabaseHelper.SaveDonor(donor);
                    count++;
                }

                Console.WriteLine($"Успешно заредени {count} тестови донора!");
                Console.WriteLine($"  - {testDonors.FindAll(d => d.DonorType == "Deceased").Count} починали донори");
                Console.WriteLine($"  - {testDonors.FindAll(d => d.DonorType == "Living").Count} живи донори");
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при зареждане на тестови данни: {ex.Message}");
            }
        }
    }
}
