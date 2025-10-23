using System;
using System.Collections.Generic;
using System.Linq;

namespace OrgnTransplant.Models
{
    public class HospitalLocation
    {
        public string Name { get; set; }
        public string City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public HospitalLocation(string name, string city, double latitude, double longitude)
        {
            Name = name;
            City = city;
            Latitude = latitude;
            Longitude = longitude;
        }

        // Get road distance using OSRM API
        public static async System.Threading.Tasks.Task<double> GetRoadDistanceAsync(HospitalLocation from, HospitalLocation to)
        {
            if (from == null || to == null)
                return -1; // Return -1 to indicate invalid input

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);

                    string url = $"https://router.project-osrm.org/route/v1/driving/{from.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{from.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)};{to.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{to.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}?overview=false";

                    var response = await client.GetStringAsync(url);

                    // Parse JSON response to get distance
                    var json = System.Text.Json.JsonDocument.Parse(response);
                    if (json.RootElement.TryGetProperty("routes", out var routes) && routes.GetArrayLength() > 0)
                    {
                        var firstRoute = routes[0];
                        if (firstRoute.TryGetProperty("distance", out var distance))
                        {
                            return distance.GetDouble() / 1000.0; // Convert meters to kilometers
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"Error getting road distance: {ex.Message}");
                // Return -1 to indicate error
                return -1;
            }

            return -1;
        }

        // Static list of all Bulgarian hospitals with GPS coordinates
        public static List<HospitalLocation> AllHospitals = new List<HospitalLocation>
        {
            // София
            new HospitalLocation("УМБАЛ Александровска - София", "София", 42.6977, 23.3219),
            new HospitalLocation("МБАЛ Св. Анна - София", "София", 42.6886, 23.3190),
            new HospitalLocation("УМБАЛ Царица Йоанна - ИСУЛ - София", "София", 42.6860, 23.3320),
            new HospitalLocation("УМБАЛ Св. Иван Рилски - София", "София", 42.6520, 23.3760),
            new HospitalLocation("УМБАЛ Св. Георги - София", "София", 42.6950, 23.3340),
            new HospitalLocation("МБАЛ Национална кардиологична болница - София", "София", 42.6820, 23.3140),
            new HospitalLocation("УМБАЛ Софиямед - София", "София", 42.6630, 23.3350),
            new HospitalLocation("СБАЛАГ Майчин дом - София", "София", 42.6970, 23.3410),
            new HospitalLocation("Военномедицинска академия - София", "София", 42.6800, 23.3630),
            new HospitalLocation("УМБАЛ Токуда Болница - София", "София", 42.6820, 23.2730),
            new HospitalLocation("Първа МБАЛ София АД - София", "София", 42.6840, 23.3250),

            // Пловдив
            new HospitalLocation("УМБАЛ Св. Георги - Пловдив", "Пловдив", 42.1354, 24.7453),
            new HospitalLocation("УМБАЛ Пулмед - Пловдив", "Пловдив", 42.1500, 24.7500),
            new HospitalLocation("МБАЛ Св. Panteleimon - Пловдив", "Пловдив", 42.1420, 24.7480),
            new HospitalLocation("УМБАЛ Каспела - Пловдив", "Пловдив", 42.1380, 24.7520),
            new HospitalLocation("УМБАЛ Св. Карidad - Пловдив", "Пловдив", 42.1400, 24.7470),

            // Варна
            new HospitalLocation("МБАЛ Св. Анна - Варна", "Варна", 43.2141, 27.9147),
            new HospitalLocation("УМБАЛ Св. Марина - Варна", "Варна", 43.2044, 27.9120),
            new HospitalLocation("МБАЛ Света Marina - Варна", "Варна", 43.2100, 27.9160),
            new HospitalLocation("УМБАЛ Медика - Варна", "Варна", 43.2070, 27.9200),

            // Бургас
            new HospitalLocation("УМБАЛ Бургас АД - Бургас", "Бургас", 42.5048, 27.4626),
            new HospitalLocation("МБАЛ Бургасмед - Бургас", "Бургас", 42.5000, 27.4650),
            new HospitalLocation("МБАЛ Св. Анна - Бургас", "Бургас", 42.5020, 27.4600),
            new HospitalLocation("МБАЛ Бургас-Медика - Бургас", "Бургас", 42.5060, 27.4680),

            // Русе
            new HospitalLocation("УМБАЛ Канев - Русе", "Русе", 43.8486, 25.9656),
            new HospitalLocation("УМБАЛ Медика - Русе", "Русе", 43.8520, 25.9700),
            new HospitalLocation("МБАЛ Русе АД - Русе", "Русе", 43.8450, 25.9620),

            // Стара Загора
            new HospitalLocation("УМБАЛ Проф. д-р Стоян Киркович - Стара Загора", "Стара Загора", 42.4258, 25.6342),
            new HospitalLocation("МБАЛ Тракия - Стара Загора", "Стара Загора", 42.4300, 25.6400),

            // Плевен
            new HospitalLocation("УМБАЛ Д-р Георги Странски - Плевен", "Плевен", 43.4170, 24.6167),
            new HospitalLocation("МБАЛ Сърце и Мозък - Плевен", "Плевен", 43.4200, 24.6200),

            // Благоевград
            new HospitalLocation("МБАЛ Св. Дух - Благоевград", "Благоевград", 42.0116, 23.0942),
            new HospitalLocation("МБАЛ Благоевград", "Благоевград", 42.0150, 23.0980),

            // Велико Търново
            new HospitalLocation("МБАЛ Д-р Стефан Черкезов - Велико Търново", "Велико Търново", 43.0757, 25.6172),
            new HospitalLocation("УМБАЛ Св. Петка - Велико Търново", "Велико Търново", 43.0800, 25.6200),

            // Шумен
            new HospitalLocation("МБАЛ Д-р Симеон Кехайов - Шумен", "Шумен", 43.2705, 26.9269),
            new HospitalLocation("УМБАЛ Д-р Панчо Владигеров - Шумен", "Шумен", 43.2750, 26.9300),

            // Сливен
            new HospitalLocation("МБАЛ Д-р Иван Селимински - Сливен", "Сливен", 42.6824, 26.3150),

            // Добрич
            new HospitalLocation("МБАЛ Добрич АД - Добрич", "Добрич", 43.5725, 27.8277),

            // Перник
            new HospitalLocation("МБАЛ Христо Ботев - Перник", "Перник", 42.6060, 23.0301),

            // Ямбол
            new HospitalLocation("МБАЛ Д-р Георги Странски - Ямбол", "Ямбол", 42.4842, 26.5036),

            // Хасково
            new HospitalLocation("МБАЛ Д-р Тота Венкова - Хасково", "Хасково", 41.9344, 25.5553),

            // Пазарджик
            new HospitalLocation("МБАЛ Д-р Иван Селимински - Пазарджик", "Пазарджик", 42.1887, 24.3332),

            // Габрово
            new HospitalLocation("МБАЛ Д-р Тота Венкова - Габрово", "Габрово", 42.8747, 25.3188),

            // Кюстендил
            new HospitalLocation("МБАЛ Пъхан - Кюстендил", "Кюстендил", 42.2858, 22.6894),

            // Кърджали
            new HospitalLocation("МБАЛ Кърджали АД - Кърджали", "Кърджали", 41.6483, 25.3678),

            // Смолян
            new HospitalLocation("МБАЛ Д-р Христо Стаменов - Смолян", "Смолян", 41.5771, 24.7010),

            // Разград
            new HospitalLocation("МБАЛ Св. Иван Рилски - Разград", "Разград", 43.5269, 26.5175),

            // Силистра
            new HospitalLocation("МБАЛ Д-р Николай Димитров Костов - Силистра", "Силистра", 44.1172, 27.2606),

            // Търговище
            new HospitalLocation("МБАЛ Д-р Христо Стаменов - Търговище", "Търговище", 43.2467, 26.5697),

            // Ловеч
            new HospitalLocation("МБАЛ Д-р Стефан Черкезов - Ловеч", "Ловеч", 43.1370, 24.7140),

            // Монтана
            new HospitalLocation("МБАЛ Д-р Стефан Черкезов - Монтана", "Монтана", 43.4092, 23.2258),

            // Видин
            new HospitalLocation("МБАЛ Света Петка - Видин", "Видин", 43.9859, 22.8778),

            // Враца
            new HospitalLocation("МБАЛ Христо Ботев - Враца", "Враца", 43.2103, 23.5489),

            // Специализирани
            new HospitalLocation("Национален кардиологичен център - София", "София", 42.6820, 23.3140),
            new HospitalLocation("Национална онкологична болница - София", "София", 42.6900, 23.3500),
            new HospitalLocation("Специализирана болница за активно лечение по нефрология - София", "София", 42.6750, 23.3450),
            new HospitalLocation("Специализирана болница за активно лечение по пневмология и фтизиатрия - София", "София", 42.6680, 23.3280)
        };

        public static HospitalLocation GetByName(string hospitalName)
        {
            return AllHospitals.FirstOrDefault(h => h.Name == hospitalName);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
