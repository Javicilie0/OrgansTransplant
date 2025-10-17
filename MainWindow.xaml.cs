using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrgnTransplant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Donor> allDonors;
        private ObservableCollection<DonorViewModel> displayedDonors;
        private AppSettings settings;
        public static HospitalLocation CurrentHospital { get; private set; }
        private bool showExpiredOrgans = false; // Toggle state for expired organs

        public MainWindow()
        {
            InitializeComponent();
            displayedDonors = new ObservableCollection<DonorViewModel>();
            DonorsDataGrid.ItemsSource = displayedDonors;

            // Load settings
            settings = AppSettings.Load();

            // Load hospitals into ComboBox
            LoadHospitals();

            // Add event handlers for real-time search
            SearchBox.TextChanged += (s, e) => ApplyFilters();
            FilterHospitalBox.SelectionChanged += (s, e) => ApplyFilters();
            FilterBloodTypeBox.SelectionChanged += (s, e) => ApplyFilters();
            FilterOrganBox.SelectionChanged += (s, e) => ApplyFilters();
        }

        private void LoadHospitals()
        {
            // Populate hospital selection
            foreach (var hospital in HospitalLocation.AllHospitals.OrderBy(h => h.City).ThenBy(h => h.Name))
            {
                HospitalSelectionBox.Items.Add(hospital);
            }

            // Load saved hospital or select first
            if (!string.IsNullOrEmpty(settings.SelectedHospital))
            {
                var savedHospital = HospitalLocation.GetByName(settings.SelectedHospital);
                if (savedHospital != null)
                {
                    HospitalSelectionBox.SelectedItem = savedHospital;
                    CurrentHospital = savedHospital;
                }
            }

            // If no hospital selected, prompt user
            if (HospitalSelectionBox.SelectedItem == null && HospitalSelectionBox.Items.Count > 0)
            {
                MessageBox.Show(
                    "Моля, изберете болницата, от която работите.\n\nТова ще помогне на системата да показва най-близките болници при търсене на органи.",
                    "Избор на болница",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void HospitalSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HospitalSelectionBox.SelectedItem is HospitalLocation hospital)
            {
                CurrentHospital = hospital;
                HospitalCityText.Text = hospital.City + ", България";

                // Save selection
                settings.SelectedHospital = hospital.Name;
                settings.SelectedHospitalCity = hospital.City;
                settings.Save();
            }
        }

        private void Organ_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string organName)
            {
                ShowOrganDetails(organName);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddWindow addWindow = new AddWindow();
                addWindow.ShowDialog();

                // Reload donors if we're on the donors view
                if (DonorsViewGrid.Visibility == Visibility.Visible)
                {
                    LoadDonors();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Грешка при отваряне на прозорец за регистрация:\n\n{ex.Message}\n\nСтек:\n{ex.StackTrace}",
                    "Грешка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void viewAll_Click(object sender, RoutedEventArgs e)
        {
            ShowOrganDetails(null); // null means show all organs
        }

        // Show Organs Details View
        private async void ShowOrganDetails(string organName)
        {
            try
            {
                OrgansViewGrid.Visibility = Visibility.Collapsed;
                DonorsViewGrid.Visibility = Visibility.Collapsed;
                OrgansDetailsGrid.Visibility = Visibility.Visible;

                // Load organs data
                List<OrganInfo> organsList = new List<OrganInfo>();

                // Get current hospital location
                var currentHospital = CurrentHospital;

                if (string.IsNullOrEmpty(organName))
                {
                    // Load all organs
                    OrganDetailsTitle.Text = "Всички налични органи за трансплантация";
                    var donors = DatabaseHelper.GetAllDonors();

                    foreach (var donor in donors)
                    {
                        if (!string.IsNullOrEmpty(donor.OrgansForDonation))
                        {
                            string[] organs = donor.OrgansForDonation.Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string organ in organs)
                            {
                                string organTrimmed = organ.Trim();
                                string iconPath = GetOrganIconPath(organTrimmed);

                                // Set distance to -1 initially (will be loaded asynchronously)
                                double distance = -1;

                                // Calculate viability information
                                string viabilityTimeDisplay = OrganViability.FormatRemainingTime(organTrimmed, donor.OrganHarvestTime);
                                string viabilityColor = OrganViability.GetViabilityColor(organTrimmed, donor.OrganHarvestTime);
                                bool isViable = OrganViability.IsOrganViable(organTrimmed, donor.OrganHarvestTime);

                                // Skip expired organs if toggle is off
                                if (!showExpiredOrgans && !isViable)
                                    continue;

                                // Format quality display
                                string qualityDisplay = "N/A";
                                if (!string.IsNullOrEmpty(donor.OrganQuality))
                                {
                                    switch (donor.OrganQuality.ToLower())
                                    {
                                        case "excellent":
                                            qualityDisplay = "⭐ Отлично";
                                            break;
                                        case "good":
                                            qualityDisplay = "✓ Добро";
                                            break;
                                        case "fair":
                                            qualityDisplay = "◐ Задоволително";
                                            break;
                                        case "poor":
                                            qualityDisplay = "✗ Лошо";
                                            break;
                                        default:
                                            qualityDisplay = donor.OrganQuality;
                                            break;
                                    }
                                }

                                organsList.Add(new OrganInfo
                                {
                                    IconPath = iconPath,
                                    OrganName = organTrimmed,
                                    Hospital = donor.Hospital ?? "N/A",
                                    Donor = donor.FullName ?? "N/A",
                                    DistanceKm = distance,
                                    AdditionalInfo = $"Blood Type: {donor.BloodType} {donor.RhFactor}\n" +
                                                   $"Phone: {donor.Phone}\n" +
                                                   $"Email: {donor.Email}\n" +
                                                   $"ЕГН: {donor.NationalId}",
                                    ViabilityTimeDisplay = viabilityTimeDisplay,
                                    ViabilityColor = viabilityColor,
                                    QualityDisplay = qualityDisplay
                                });
                            }
                        }
                    }
                }
                else
                {
                    // Load specific organ
                    OrganDetailsTitle.Text = $"Налични донори за {organName}";
                    var donors = DatabaseHelper.GetDonorsByOrgan(organName);

                    foreach (var donor in donors)
                    {
                        string iconPath = GetOrganIconPath(organName);

                        // Set distance to -1 initially (will be loaded asynchronously)
                        double distance = -1;

                        // Calculate viability information
                        string viabilityTimeDisplay = OrganViability.FormatRemainingTime(organName, donor.OrganHarvestTime);
                        string viabilityColor = OrganViability.GetViabilityColor(organName, donor.OrganHarvestTime);
                        bool isViable = OrganViability.IsOrganViable(organName, donor.OrganHarvestTime);

                        // Skip expired organs if toggle is off
                        if (!showExpiredOrgans && !isViable)
                            continue;

                        // Format quality display
                        string qualityDisplay = "N/A";
                        if (!string.IsNullOrEmpty(donor.OrganQuality))
                        {
                            switch (donor.OrganQuality.ToLower())
                            {
                                case "excellent":
                                    qualityDisplay = "⭐ Отлично";
                                    break;
                                case "good":
                                    qualityDisplay = "✓ Добро";
                                    break;
                                case "fair":
                                    qualityDisplay = "◐ Задоволително";
                                    break;
                                case "poor":
                                    qualityDisplay = "✗ Лошо";
                                    break;
                                default:
                                    qualityDisplay = donor.OrganQuality;
                                    break;
                            }
                        }

                        organsList.Add(new OrganInfo
                        {
                            IconPath = iconPath,
                            OrganName = organName,
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

                // Update UI immediately with organs (distances will be N/A initially)
                OrgansItemsControl.ItemsSource = organsList;
                TotalOrgansText.Text = organsList.Count.ToString();

                // Load distances asynchronously in the background
                if (currentHospital != null)
                {
                    _ = LoadDistancesAsync(organsList, currentHospital);
                }

                if (organsList.Count > 0)
                {
                    var closestOrgan = organsList.FirstOrDefault(o => o.DistanceKm >= 0);
                    if (closestOrgan != null)
                    {
                        ClosestDistanceText.Text = $"{closestOrgan.DistanceKm:F0} км";
                    }
                    else
                    {
                        ClosestDistanceText.Text = "N/A";
                    }
                }
                else
                {
                    ClosestDistanceText.Text = "N/A";
                    MessageBox.Show(
                        string.IsNullOrEmpty(organName)
                            ? "Няма регистрирани донори в системата."
                            : $"Няма налични донори за {organName}.",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане на органи: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load distances asynchronously for all organs
        private async System.Threading.Tasks.Task LoadDistancesAsync(List<OrganInfo> organsList, HospitalLocation currentHospital)
        {
            try
            {
                // Load distances in parallel
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

                await System.Threading.Tasks.Task.WhenAll(tasks);

                // Sort by distance and update UI on the main thread
                await Dispatcher.InvokeAsync(() =>
                {
                    var sortedList = organsList.OrderBy(o => o.DistanceKm < 0 ? double.MaxValue : o.DistanceKm).ToList();
                    OrgansItemsControl.ItemsSource = null;
                    OrgansItemsControl.ItemsSource = sortedList;

                    // Update closest distance
                    var closestOrgan = sortedList.FirstOrDefault(o => o.DistanceKm >= 0);
                    if (closestOrgan != null)
                    {
                        ClosestDistanceText.Text = $"{closestOrgan.DistanceKm:F0} км";
                    }
                    else
                    {
                        ClosestDistanceText.Text = "N/A";
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading distances: {ex.Message}");
            }
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

        // Show Donors View
        private void ViewDonors_Click(object sender, RoutedEventArgs e)
        {
            OrgansViewGrid.Visibility = Visibility.Collapsed;
            OrgansDetailsGrid.Visibility = Visibility.Collapsed;
            DonorsViewGrid.Visibility = Visibility.Visible;
            LoadDonors();
        }

        // Show Messages Window
        private void ViewMessages_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentHospital == null)
            {
                MessageBox.Show(
                    "Моля, първо изберете вашата болница от горното меню.",
                    "Изберете болница",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MessagesWindow messagesWindow = new MessagesWindow(CurrentHospital.Name);
            messagesWindow.ShowDialog();
        }

        // Back to Organs View
        private void BackToOrgans_Click(object sender, RoutedEventArgs e)
        {
            DonorsViewGrid.Visibility = Visibility.Collapsed;
            OrgansDetailsGrid.Visibility = Visibility.Collapsed;
            OrgansViewGrid.Visibility = Visibility.Visible;
        }

        // Load all donors from database
        private void LoadDonors()
        {
            try
            {
                allDonors = DatabaseHelper.GetAllDonors();

                // Get unique hospitals for filter
                var hospitals = allDonors.Select(d => d.Hospital).Distinct().OrderBy(h => h).ToList();
                FilterHospitalBox.Items.Clear();
                FilterHospitalBox.Items.Add(new ComboBoxItem { Content = "Всички болници", IsSelected = true });
                foreach (var hospital in hospitals)
                {
                    if (!string.IsNullOrEmpty(hospital))
                    {
                        FilterHospitalBox.Items.Add(new ComboBoxItem { Content = hospital });
                    }
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане на донори: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Apply search and filters
        private void ApplyFilters()
        {
            if (allDonors == null) return;

            var filtered = allDonors.AsEnumerable();

            // Search by name
            string searchText = SearchBox?.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(d => d.FullName.ToLower().Contains(searchText));
            }

            // Filter by hospital
            var selectedHospital = (FilterHospitalBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedHospital) && selectedHospital != "Всички болници")
            {
                filtered = filtered.Where(d => d.Hospital == selectedHospital);
            }

            // Filter by blood type
            var selectedBloodType = (FilterBloodTypeBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedBloodType) && selectedBloodType != "Всички")
            {
                string bloodType = selectedBloodType.Substring(0, selectedBloodType.Length - 1); // Remove +/-
                string rh = selectedBloodType.EndsWith("+") ? "Положителен" : "Отрицателен";
                filtered = filtered.Where(d => d.BloodType == bloodType && d.RhFactor == rh);
            }

            // Filter by organ
            var selectedOrgan = (FilterOrganBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedOrgan) && selectedOrgan != "Всички органи")
            {
                filtered = filtered.Where(d => d.OrgansForDonation?.Contains(selectedOrgan) == true);
            }

            // Update DataGrid
            displayedDonors.Clear();
            foreach (var donor in filtered)
            {
                displayedDonors.Add(new DonorViewModel(donor));
            }
        }

        // Search button click
        private void SearchDonors_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        // Edit donor
        private void EditDonor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                try
                {
                    int donorId = Convert.ToInt32(btn.Tag);
                    AddWindow editWindow = new AddWindow(donorId);
                    editWindow.ShowDialog();

                    // After edit, reload donors
                    LoadDonors();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Грешка при отваряне на прозорец за редакция: {ex.Message}",
                        "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Delete donor
        private void DeleteDonor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int donorId = Convert.ToInt32(btn.Tag);
                var donor = allDonors.FirstOrDefault(d => d.Id == donorId);

                if (donor != null)
                {
                    var result = MessageBox.Show(
                        $"Сигурни ли сте, че искате да изтриете донор:\n\n{donor.FullName}\nЕГН: {donor.NationalId}\n\nТова действие не може да бъде отменено!",
                        "Потвърждение за изтриване",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            bool success = DatabaseHelper.DeleteDonor(donorId);
                            if (success)
                            {
                                MessageBox.Show("Донорът беше изтрит успешно!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadDonors(); // Reload the list
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Грешка при изтриване: {ex.Message}",
                                "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        // Seed test data using TestDataSeeder class
        private void SeedTestData_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Това ще зареди базата данни с 25 тестови донора.\n\n" +
                "ВАЖНО: Базата данни трябва да е празна!\n\n" +
                "Ако има съществуващи записи, изтрийте ги първо.\n\n" +
                "Искате ли да продължите?",
                "Зареждане на тестови данни",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TestDataSeeder.SeedTestData();
                    MessageBox.Show(
                        "Тестовите данни бяха заредени успешно!\n\n" +
                        "✅ Заредени: 25 донора\n" +
                        "📍 Градове: София, Пловдив, Варна, Бургас, Русе и др.\n" +
                        "🫀 Всички органи включени\n" +
                        "🩸 Всички кръвни групи",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Грешка при зареждане на данни:\n\n{ex.Message}\n\n" +
                        "💡 Съвет: Ако базата не е празна, изтрийте записите първо.",
                        "Грешка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Clear all data from database
        private void ClearDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ ВНИМАНИЕ! ⚠️\n\n" +
                "Това действие ще изтрие ВСИЧКИ донори и съобщения от базата данни!\n\n" +
                "Това действие НЕ МОЖЕ да бъде отменено!\n\n" +
                "Сигурни ли сте, че искате да продължите?",
                "Изтриване на всички данни",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Second confirmation
                var secondConfirm = MessageBox.Show(
                    "Това е последната проверка!\n\n" +
                    "Натиснете ДА за окончателно изтриване на всички данни.",
                    "Потвърждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation);

                if (secondConfirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Delete all donors
                        using (var connection = DatabaseHelper.GetConnection())
                        {
                            connection.Open();

                            // Delete all messages first (foreign key constraint)
                            using (var cmd = new MySqlCommand("DELETE FROM messages", connection))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            // Delete all donors
                            using (var cmd = new MySqlCommand("DELETE FROM Donors", connection))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }

                        MessageBox.Show(
                            "✅ Всички данни бяха изтрити успешно!\n\n" +
                            "Базата данни е празна и готова за нови данни.",
                            "Успех",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the current view
                        if (DonorsViewGrid.Visibility == Visibility.Visible)
                        {
                            LoadDonors();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Грешка при изтриване на данни:\n\n{ex.Message}",
                            "Грешка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        // Request organ from donor
        private void RequestOrgan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if hospital is selected
                if (CurrentHospital == null)
                {
                    MessageBox.Show(
                        "Моля, първо изберете вашата болница от горното меню.",
                        "Изберете болница",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Get organ info from button tag
                if (sender is Button btn && btn.Tag is OrganInfo organInfo)
                {
                    // Get donor hospital
                    var donorHospital = HospitalLocation.GetByName(organInfo.Hospital);
                    if (donorHospital == null)
                    {
                        MessageBox.Show(
                            "Болницата на донора не може да бъде намерена в системата.",
                            "Грешка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    // Show confirmation dialog
                    var result = MessageBox.Show(
                        $"Заявка за орган\n\n" +
                        $"🫀 Орган: {organInfo.OrganName}\n" +
                        $"👤 Донор: {organInfo.Donor}\n" +
                        $"🏥 От болница: {organInfo.Hospital}\n" +
                        $"📍 Разстояние: {organInfo.DistanceDisplay}\n\n" +
                        $"Искате ли да изпратите заявка до {organInfo.Hospital}?",
                        "Потвърждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Create and send request message
                        Message requestMessage = new Message
                        {
                            FromHospital = CurrentHospital.Name,
                            ToHospital = organInfo.Hospital,
                            OrganName = organInfo.OrganName,
                            DonorName = organInfo.Donor,
                            MessageType = MessageType.Request,
                            Status = MessageStatus.Pending,
                            MessageText = $"Заявка за {organInfo.OrganName} от {CurrentHospital.Name}. Разстояние между болниците: {organInfo.DistanceDisplay}.",
                            DeliveryOption = DeliveryOption.NotSpecified,
                            CreatedAt = DateTime.Now
                        };

                        // Send to database
                        MessagesHelper.SendRequest(requestMessage);

                        MessageBox.Show(
                            "✅ Заявката беше изпратена успешно!\n\n" +
                            "Можете да проверите статуса й в секция 📬 Поща.",
                            "Успех",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Грешка при изпращане на заявка:\n\n{ex.Message}",
                    "Грешка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Show map for organ transport route
        private void ShowMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if hospital is selected
                if (CurrentHospital == null)
                {
                    MessageBox.Show(
                        "Моля, първо изберете вашата болница от горното меню.",
                        "Изберете болница",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Get organ info from button tag
                if (sender is Button btn && btn.Tag is OrganInfo organInfo)
                {
                    // Get donor hospital location
                    var donorHospital = HospitalLocation.GetByName(organInfo.Hospital);
                    if (donorHospital == null)
                    {
                        MessageBox.Show(
                            "Болницата на донора не може да бъде намерена в системата.",
                            "Грешка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    // Calculate organ viability hours from ViabilityTimeDisplay
                    double organViabilityHours = 12; // Default value
                    var viabilityTimeMatch = System.Text.RegularExpressions.Regex.Match(
                        organInfo.ViabilityTimeDisplay,
                        @"(\d+(\.\d+)?)\s*(ч|часа)");

                    if (viabilityTimeMatch.Success)
                    {
                        double.TryParse(viabilityTimeMatch.Groups[1].Value, out organViabilityHours);
                    }

                    // Open map window
                    MapWindow mapWindow = new MapWindow(
                        donorHospital.Latitude,
                        donorHospital.Longitude,
                        donorHospital.Name,
                        CurrentHospital.Latitude,
                        CurrentHospital.Longitude,
                        CurrentHospital.Name,
                        organViabilityHours
                    );

                    mapWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Грешка при отваряне на карта:\n\n{ex.Message}",
                    "Грешка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Toggle showing expired organs
        private void ShowExpiredToggle_Click(object sender, RoutedEventArgs e)
        {
            showExpiredOrgans = !showExpiredOrgans;

            // Update button appearance and text
            if (showExpiredOrgans)
            {
                ShowExpiredToggle.Content = "🚫 Скрий изтекли органи";
                ShowExpiredToggle.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E74C3C"));
            }
            else
            {
                ShowExpiredToggle.Content = "👁️ Покажи изтекли органи";
                ShowExpiredToggle.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#95A5A6"));
            }

            // Reload the current view to apply filter
            if (OrgansDetailsGrid.Visibility == Visibility.Visible)
            {
                // Get the current organ name from the title
                string currentTitle = OrganDetailsTitle.Text;
                if (currentTitle.Contains("Всички"))
                {
                    ShowOrganDetails(null); // Reload all organs
                }
                else
                {
                    // Extract organ name from title "Налични донори за X"
                    string organName = currentTitle.Replace("Налични донори за ", "").Trim();
                    ShowOrganDetails(organName);
                }
            }
        }
    }

    // ViewModel for DataGrid display
    public class DonorViewModel
    {
        private Donor donor;

        public DonorViewModel(Donor donor)
        {
            this.donor = donor;
        }

        public int Id => donor.Id;
        public string FullName => donor.FullName;

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - donor.DateOfBirth.Year;
                if (donor.DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        // Masked EGN (9501****** for security)
        public string MaskedEGN
        {
            get
            {
                if (string.IsNullOrEmpty(donor.NationalId) || donor.NationalId.Length < 10)
                    return donor.NationalId;

                return donor.NationalId.Substring(0, 4) + "******";
            }
        }

        public string BloodTypeDisplay
        {
            get
            {
                string rh = donor.RhFactor == "Положителен" ? "+" : "-";
                return $"{donor.BloodType}{rh}";
            }
        }

        public string Hospital => donor.Hospital;
        public string OrgansForDonation => donor.OrgansForDonation;
        public DateTime DateOfBirth => donor.DateOfBirth;

        // Display organ harvest time
        public string HarvestTimeDisplay
        {
            get
            {
                if (donor.OrganHarvestTime == DateTime.MinValue)
                    return "N/A";

                TimeSpan elapsed = DateTime.Now - donor.OrganHarvestTime;
                if (elapsed.TotalHours < 1)
                    return $"Преди {(int)elapsed.TotalMinutes} мин";
                else if (elapsed.TotalHours < 24)
                    return $"Преди {(int)elapsed.TotalHours} ч";
                else if (elapsed.TotalDays < 2)
                    return "Преди 1 ден";
                else
                    return $"Преди {(int)elapsed.TotalDays} дни";
            }
        }

        // Display organ quality
        public string QualityDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(donor.OrganQuality))
                    return "N/A";

                switch (donor.OrganQuality.ToLower())
                {
                    case "excellent":
                        return "⭐ Отлично";
                    case "good":
                        return "✓ Добро";
                    case "fair":
                        return "◐ Задоволително";
                    case "poor":
                        return "✗ Лошо";
                    default:
                        return donor.OrganQuality;
                }
            }
        }
    }
}
