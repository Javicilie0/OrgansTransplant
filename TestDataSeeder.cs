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

                List<Donor> testDonors = new List<Donor>
                {
                    // Донори от София
                    new Donor {
                        FullName = "Иван Петров Георгиев",
                        Hospital = "УМБАЛ Александровска - София",
                        DateOfBirth = new DateTime(1985, 3, 15),
                        Gender = "Мъж",
                        NationalId = "8503154321",
                        Phone = "0888123456",
                        Email = "ivan.petrov@email.com",
                        Address = "ул. Витоша 15, София",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Сърце, Бял дроб"
                    },
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
                        OrgansForDonation = "Бъбрек"
                    },
                    new Donor {
                        FullName = "Георги Стефанов Николов",
                        Hospital = "Военномедицинска академия - София",
                        DateOfBirth = new DateTime(1978, 11, 8),
                        Gender = "Мъж",
                        NationalId = "7811083456",
                        Phone = "0889111222",
                        Email = "georgi.stefanov@email.com",
                        Address = "ул. Цар Борис 23, София",
                        BloodType = "B",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черен дроб, Панкреас"
                    },

                    // Донори от Пловдив
                    new Donor {
                        FullName = "Димитър Христов Петков",
                        Hospital = "УМБАЛ Св. Георги - Пловдив",
                        DateOfBirth = new DateTime(1988, 5, 12),
                        Gender = "Мъж",
                        NationalId = "8805126789",
                        Phone = "0886222333",
                        Email = "dimitar.hristov@email.com",
                        Address = "ул. Марица 67, Пловдив",
                        BloodType = "A",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бъбрек, Бял дроб"
                    },
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
                        OrgansForDonation = "Сърце"
                    },

                    // Донори от Варна
                    new Donor {
                        FullName = "Стефан Атанасов Кирилов",
                        Hospital = "УМБАЛ Св. Марина - Варна",
                        DateOfBirth = new DateTime(1983, 12, 25),
                        Gender = "Мъж",
                        NationalId = "8312259012",
                        Phone = "0884444555",
                        Email = "stefan.atanasov@email.com",
                        Address = "ул. Черно море 89, Варна",
                        BloodType = "O",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черен дроб"
                    },
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
                        OrgansForDonation = "Бъбрек, Панкреас"
                    },

                    // Донори от Бургас
                    new Donor {
                        FullName = "Николай Василев Стоянов",
                        Hospital = "УМБАЛ Бургас АД - Бургас",
                        DateOfBirth = new DateTime(1987, 8, 7),
                        Gender = "Мъж",
                        NationalId = "8708072345",
                        Phone = "0882666777",
                        Email = "nikolay.vasilev@email.com",
                        Address = "ул. Александровска 34, Бургас",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Сърце, Бял дроб"
                    },
                    new Donor {
                        FullName = "Десислава Петкова Иванова",
                        Hospital = "МБАЛ Бургасмед - Бургас",
                        DateOfBirth = new DateTime(1993, 2, 14),
                        Gender = "Жена",
                        NationalId = "9302143456",
                        Phone = "0881777888",
                        Email = "desislava.petkova@email.com",
                        Address = "бул. Демокрация 78, Бургас",
                        BloodType = "O",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бъбрек"
                    },

                    // Донори от Русе
                    new Donor {
                        FullName = "Петър Димитров Стефанов",
                        Hospital = "УМБАЛ Канев - Русе",
                        DateOfBirth = new DateTime(1981, 6, 20),
                        Gender = "Мъж",
                        NationalId = "8106204567",
                        Phone = "0880888999",
                        Email = "petar.dimitrov@email.com",
                        Address = "ул. Дунавска 56, Русе",
                        BloodType = "AB",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черен дроб, Панкреас"
                    },
                    new Donor {
                        FullName = "Мариана Христова Георгиева",
                        Hospital = "УМБАЛ Медика - Русе",
                        DateOfBirth = new DateTime(1994, 10, 5),
                        Gender = "Жена",
                        NationalId = "9410055678",
                        Phone = "0889000111",
                        Email = "mariana.hristova@email.com",
                        Address = "бул. Липник 23, Русе",
                        BloodType = "B",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Сърце, Бъбрек"
                    },

                    // Донори от Стара Загора
                    new Donor {
                        FullName = "Васил Тодоров Николов",
                        Hospital = "УМБАЛ Проф. д-р Стоян Киркович - Стара Загора",
                        DateOfBirth = new DateTime(1986, 1, 28),
                        Gender = "Мъж",
                        NationalId = "8601286789",
                        Phone = "0888222333",
                        Email = "vasil.todorov@email.com",
                        Address = "ул. Цар Симеон 12, Стара Загора",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бял дроб, Тимус"
                    },
                    new Donor {
                        FullName = "Надежда Атанасова Димитрова",
                        Hospital = "МБАЛ Тракия - Стара Загора",
                        DateOfBirth = new DateTime(1991, 3, 17),
                        Gender = "Жена",
                        NationalId = "9103177890",
                        Phone = "0887333444",
                        Email = "nadejda.atanasova@email.com",
                        Address = "бул. Патриарх Евтимий 45, Стара Загора",
                        BloodType = "O",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бъбрек"
                    },

                    // Донори от Плевен
                    new Donor {
                        FullName = "Христо Георгиев Петров",
                        Hospital = "УМБАЛ Д-р Георги Странски - Плевен",
                        DateOfBirth = new DateTime(1984, 9, 11),
                        Gender = "Мъж",
                        NationalId = "8409118901",
                        Phone = "0886444555",
                        Email = "hristo.georgiev@email.com",
                        Address = "ул. Васил Левски 67, Плевен",
                        BloodType = "B",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черен дроб"
                    },
                    new Donor {
                        FullName = "Йорданка Николова Стефанова",
                        Hospital = "МБАЛ Сърце и Мозък - Плевен",
                        DateOfBirth = new DateTime(1989, 12, 3),
                        Gender = "Жена",
                        NationalId = "8912039012",
                        Phone = "0885555666",
                        Email = "yordanka.nikolova@email.com",
                        Address = "бул. Русе 89, Плевен",
                        BloodType = "AB",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Сърце, Панкреас"
                    },

                    // Допълнителни донори
                    new Donor {
                        FullName = "Александър Стоянов Димитров",
                        Hospital = "УМБАЛ Св. Петка - Велико Търново",
                        DateOfBirth = new DateTime(1982, 7, 15),
                        Gender = "Мъж",
                        NationalId = "8207151234",
                        Phone = "0884666777",
                        Email = "aleksandar.stoyanov@email.com",
                        Address = "ул. Цар Асен 34, Велико Търново",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бъбрек, Бял дроб"
                    },
                    new Donor {
                        FullName = "Силвия Петрова Христова",
                        Hospital = "МБАЛ Д-р Стефан Черкезов - Велико Търново",
                        DateOfBirth = new DateTime(1996, 11, 22),
                        Gender = "Жена",
                        NationalId = "9611222345",
                        Phone = "0883777888",
                        Email = "silviya.petrova@email.com",
                        Address = "бул. България 12, Велико Търново",
                        BloodType = "O",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черен дроб"
                    },
                    new Donor {
                        FullName = "Тодор Иванов Георгиев",
                        Hospital = "УМБАЛ Д-р Панчо Владигеров - Шумен",
                        DateOfBirth = new DateTime(1980, 4, 9),
                        Gender = "Мъж",
                        NationalId = "8004093456",
                        Phone = "0882888999",
                        Email = "todor.ivanov@email.com",
                        Address = "ул. Мадара 56, Шумен",
                        BloodType = "B",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Сърце, Стомах"
                    },
                    new Donor {
                        FullName = "Красимира Димитрова Василева",
                        Hospital = "МБАЛ Д-р Симеон Кехайов - Шумен",
                        DateOfBirth = new DateTime(1993, 8, 25),
                        Gender = "Жена",
                        NationalId = "9308254567",
                        Phone = "0881999000",
                        Email = "krasimira.dimitrova@email.com",
                        Address = "бул. Славянски 78, Шумен",
                        BloodType = "A",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бъбрек"
                    },
                    new Donor {
                        FullName = "Борислав Петков Николов",
                        Hospital = "МБАЛ Добрич АД - Добрич",
                        DateOfBirth = new DateTime(1987, 2, 28),
                        Gender = "Мъж",
                        NationalId = "8702285678",
                        Phone = "0880111222",
                        Email = "borislav.petkov@email.com",
                        Address = "ул. Добруджа 23, Добрич",
                        BloodType = "O",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черен дроб, Панкреас, Артерия"
                    },
                    new Donor {
                        FullName = "Даниела Стефанова Тодорова",
                        Hospital = "УМБАЛ Царица Йоанна - ИСУЛ - София",
                        DateOfBirth = new DateTime(1990, 6, 12),
                        Gender = "Жена",
                        NationalId = "9006126789",
                        Phone = "0889333444",
                        Email = "daniela.stefanova@email.com",
                        Address = "ул. Студентски град 45, София",
                        BloodType = "AB",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Сърце, Бъбрек, Черен дроб"
                    },
                    new Donor {
                        FullName = "Емил Василев Атанасов",
                        Hospital = "УМБАЛ Св. Иван Рилски - София",
                        DateOfBirth = new DateTime(1985, 10, 20),
                        Gender = "Мъж",
                        NationalId = "8510207890",
                        Phone = "0888444555",
                        Email = "emil.vasilev@email.com",
                        Address = "бул. Драган Цанков 12, София",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бял дроб, Тимус"
                    },
                    new Donor {
                        FullName = "Ивелина Христова Петкова",
                        Hospital = "УМБАЛ Каспела - Пловдив",
                        DateOfBirth = new DateTime(1992, 1, 15),
                        Gender = "Жена",
                        NationalId = "9201158901",
                        Phone = "0887555666",
                        Email = "ivelina.hristova@email.com",
                        Address = "ул. Христо Ботев 34, Пловдив",
                        BloodType = "B",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Бъбрек, Панкреас"
                    },
                    new Donor {
                        FullName = "Радослав Георгиев Стоянов",
                        Hospital = "УМБАЛ Медика - Варна",
                        DateOfBirth = new DateTime(1988, 3, 8),
                        Gender = "Мъж",
                        NationalId = "8803089012",
                        Phone = "0886666777",
                        Email = "radoslav.georgiev@email.com",
                        Address = "ул. Цар Освободител 67, Варна",
                        BloodType = "O",
                        RhFactor = "Отрицателен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Черва, Стомах"
                    },
                    new Donor {
                        FullName = "Теодора Николова Димитрова",
                        Hospital = "МБАЛ Св. Анна - Бургас",
                        DateOfBirth = new DateTime(1994, 7, 30),
                        Gender = "Жена",
                        NationalId = "9407301234",
                        Phone = "0885777888",
                        Email = "teodora.nikolova@email.com",
                        Address = "бул. Христо Ботев 89, Бургас",
                        BloodType = "A",
                        RhFactor = "Положителен",
                        InfectiousDiseases = "Няма",
                        OrgansForDonation = "Артерия, Тимус"
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при зареждане на тестови данни: {ex.Message}");
            }
        }
    }
}
