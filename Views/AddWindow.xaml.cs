using System;
using OrgnTransplant.Models;
using OrgnTransplant.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace OrgnTransplant.Views
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private Donor _editingDonor;
        private bool _isEditMode;

        // Constructor for adding new donor
        public AddWindow()
        {
            InitializeComponent();
            _isEditMode = false;

            this.Title = "Регистрация на нов донор";

            // Add event handlers for real-time validation
            FullNameBox.TextChanged += FullNameBox_TextChanged;
            NationalIdBox.TextChanged += NationalIdBox_TextChanged;
            PhoneBox.TextChanged += PhoneBox_TextChanged;
            EmailBox.TextChanged += EmailBox_TextChanged;
            AddressBox.TextChanged += AddressBox_TextChanged;
            FamilyConsentBox.TextChanged += FamilyConsentBox_TextChanged;
            FamilyContactPhoneBox.TextChanged += FamilyContactPhoneBox_TextChanged;
            NationalIdBox.PreviewTextInput += NumericOnly_PreviewTextInput;
            PhoneBox.PreviewTextInput += NumericOnly_PreviewTextInput;
            FamilyContactPhoneBox.PreviewTextInput += NumericOnly_PreviewTextInput;

            // Add event handlers for auto-formatting dates
            DOBBox.TextChanged += DateBox_TextChanged;
            DateOfDeathBox.TextChanged += DateBox_TextChanged;
            HarvestDateBox.TextChanged += DateBox_TextChanged;

            // Add event handlers for auto-formatting times
            DeathTimeBox.TextChanged += TimeBox_TextChanged;
            HarvestTimeBox.TextChanged += TimeBox_TextChanged;

            // Set default donor type visibility (AFTER InitializeComponent)
            UpdateFieldVisibilityBasedOnDonorType();
        }

        // Constructor for editing existing donor
        public AddWindow(int donorId) : this()
        {
            _isEditMode = true;
            this.Title = "Редактиране на донор";

            // Load donor data
            try
            {
                _editingDonor = DatabaseHelper.GetDonorById(donorId);
                if (_editingDonor != null)
                {
                    LoadDonorData(_editingDonor);
                }
                else
                {
                    MessageBox.Show("Донорът не може да бъде намерен!", "Грешка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане на данни: {ex.Message}", "Грешка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        // Load donor data into form fields
        private void LoadDonorData(Donor donor)
        {
            FullNameBox.Text = donor.FullName;
            DOBBox.Text = donor.DateOfBirth.ToString("dd/MM/yyyy");
            NationalIdBox.Text = donor.NationalId;
            PhoneBox.Text = donor.Phone;
            EmailBox.Text = donor.Email;
            AddressBox.Text = donor.Address;
            InfectiousBox.Text = donor.InfectiousDiseases;

            // Set gender
            foreach (ComboBoxItem item in GenderBox.Items)
            {
                if (item.Content.ToString() == donor.Gender)
                {
                    GenderBox.SelectedItem = item;
                    break;
                }
            }

            // Set blood type
            foreach (ComboBoxItem item in BloodTypeBox.Items)
            {
                if (item.Content.ToString() == donor.BloodType)
                {
                    BloodTypeBox.SelectedItem = item;
                    break;
                }
            }

            // Set Rh factor
            foreach (ComboBoxItem item in RhFactorBox.Items)
            {
                if (item.Content.ToString() == donor.RhFactor)
                {
                    RhFactorBox.SelectedItem = item;
                    break;
                }
            }

            // Set selected organs
            if (!string.IsNullOrEmpty(donor.OrgansForDonation))
            {
                string[] organs = donor.OrgansForDonation.Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (CheckBox cb in OrganPanel.Children.OfType<CheckBox>())
                {
                    if (organs.Contains(cb.Content.ToString()))
                    {
                        cb.IsChecked = true;
                    }
                }
            }

            // Set organ harvest time
            if (donor.OrganHarvestTime != DateTime.MinValue)
            {
                HarvestDateBox.Text = donor.OrganHarvestTime.ToString("dd/MM/yyyy");
                HarvestTimeBox.Text = donor.OrganHarvestTime.ToString("HH:mm");
            }

            // Set organ quality (matching Tag with English value from database)
            if (!string.IsNullOrEmpty(donor.OrganQuality))
            {
                foreach (ComboBoxItem item in OrganQualityBox.Items)
                {
                    if (item.Tag?.ToString().Equals(donor.OrganQuality, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        OrganQualityBox.SelectedItem = item;
                        break;
                    }
                }
            }

            // Load deceased donor fields
            if (donor.DonorType == "Deceased")
            {
                DeceasedDonorRadio.IsChecked = true;

                if (donor.DateOfDeath.HasValue)
                {
                    DateOfDeathBox.Text = donor.DateOfDeath.Value.ToString("dd/MM/yyyy");
                    DeathTimeBox.Text = donor.DateOfDeath.Value.ToString("HH:mm");
                }

                CauseOfDeathBox.Text = donor.CauseOfDeath;
                FamilyConsentBox.Text = donor.FamilyConsentGivenBy;
                FamilyContactPhoneBox.Text = donor.FamilyContactPhone;

                // Set family relationship
                foreach (ComboBoxItem item in FamilyRelationshipBox.Items)
                {
                    if (item.Content.ToString() == donor.FamilyRelationship)
                    {
                        FamilyRelationshipBox.SelectedItem = item;
                        break;
                    }
                }

                // If "Друг роднина", show the text field
                if (donor.FamilyRelationship == "Друг роднина" && !string.IsNullOrEmpty(donor.FamilyRelationship))
                {
                    FamilyRelationshipOtherLabel.Visibility = Visibility.Visible;
                    FamilyRelationshipOtherBox.Visibility = Visibility.Visible;
                    FamilyRelationshipOtherBox.Text = donor.FamilyRelationship;
                }
            }
            else
            {
                LivingDonorRadio.IsChecked = true;
            }

            AdditionalNotesBox.Text = donor.AdditionalNotes;
            RegisteredByBox.Text = donor.RegisteredBy;
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Handle donor type change
        private void DonorType_Changed(object sender, RoutedEventArgs e)
        {
            UpdateFieldVisibilityBasedOnDonorType();
        }

        // Update field visibility based on donor type
        private void UpdateFieldVisibilityBasedOnDonorType()
        {
            // Safety check - make sure all controls are initialized
            if (DeceasedDonorPanel == null || OrganViabilityPanel == null ||
                EmailLabel == null || EmailBox == null || DeceasedDonorRadio == null)
            {
                return;
            }

            bool isDeceased = DeceasedDonorRadio.IsChecked == true;

            if (isDeceased)
            {
                // Show deceased donor fields
                DeceasedDonorPanel.Visibility = Visibility.Visible;
                OrganViabilityPanel.Visibility = Visibility.Visible;

                // Hide non-essential fields for deceased donor
                EmailLabel.Visibility = Visibility.Collapsed;
                EmailBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Hide deceased donor fields
                DeceasedDonorPanel.Visibility = Visibility.Collapsed;
                OrganViabilityPanel.Visibility = Visibility.Collapsed;

                // Show all fields for living donor
                EmailLabel.Visibility = Visibility.Visible;
                EmailBox.Visibility = Visibility.Visible;
            }
        }

        // Handle infectious diseases "Other" selection
        private void InfectiousBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InfectiousBox.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Content.ToString() == "Друго")
                {
                    InfectiousOtherLabel.Visibility = Visibility.Visible;
                    InfectiousOtherBox.Visibility = Visibility.Visible;
                }
                else
                {
                    InfectiousOtherLabel.Visibility = Visibility.Collapsed;
                    InfectiousOtherBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Handle family relationship "Other" selection
        private void FamilyRelationshipBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FamilyRelationshipBox.SelectedItem is ComboBoxItem selected)
            {
                if (selected.Content.ToString() == "Друг роднина")
                {
                    FamilyRelationshipOtherLabel.Visibility = Visibility.Visible;
                    FamilyRelationshipOtherBox.Visibility = Visibility.Visible;
                }
                else
                {
                    FamilyRelationshipOtherLabel.Visibility = Visibility.Collapsed;
                    FamilyRelationshipOtherBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Only allow numeric input
        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }

        // Auto-format date input (dd/mm/yyyy)
        private void DateBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Remove the event handler temporarily to prevent infinite loop
                textBox.TextChanged -= DateBox_TextChanged;

                string text = textBox.Text.Replace("/", ""); // Remove existing slashes
                int cursorPosition = textBox.SelectionStart;

                // Only format if all characters are digits
                if (text.Length > 0 && text.All(char.IsDigit))
                {
                    string formatted = "";

                    // Limit to 8 digits max (ddmmyyyy)
                    if (text.Length > 8)
                        text = text.Substring(0, 8);

                    // Add first 2 digits (day)
                    if (text.Length >= 1)
                        formatted += text.Substring(0, Math.Min(2, text.Length));

                    // Add slash after day
                    if (text.Length > 2)
                        formatted += "/" + text.Substring(2, Math.Min(2, text.Length - 2));

                    // Add slash after month
                    if (text.Length > 4)
                        formatted += "/" + text.Substring(4);

                    textBox.Text = formatted;

                    // Move cursor to end
                    textBox.SelectionStart = formatted.Length;
                }
                else if (text.Length == 0)
                {
                    // Allow empty text
                    textBox.Text = "";
                }

                // Re-add the event handler
                textBox.TextChanged += DateBox_TextChanged;
            }
        }

        // Auto-format time input (hh:mm)
        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Remove the event handler temporarily to prevent infinite loop
                textBox.TextChanged -= TimeBox_TextChanged;

                string text = textBox.Text.Replace(":", ""); // Remove existing colons
                int cursorPosition = textBox.SelectionStart;

                // Only format if all characters are digits
                if (text.Length > 0 && text.All(char.IsDigit))
                {
                    string formatted = "";

                    // Limit to 4 digits max (hhmm)
                    if (text.Length > 4)
                        text = text.Substring(0, 4);

                    // Add first 2 digits (hour)
                    if (text.Length >= 1)
                        formatted += text.Substring(0, Math.Min(2, text.Length));

                    // Add colon after hour
                    if (text.Length > 2)
                        formatted += ":" + text.Substring(2);

                    textBox.Text = formatted;

                    // Move cursor to end
                    textBox.SelectionStart = formatted.Length;
                }
                else if (text.Length == 0)
                {
                    // Allow empty text
                    textBox.Text = "";
                }

                // Re-add the event handler
                textBox.TextChanged += TimeBox_TextChanged;
            }
        }

        // Validate full name (3 names with spaces)
        private void FullNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = FullNameBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                FullNameBox.Background = Brushes.White;
                FullNameBox.ToolTip = null;
                return;
            }

            // Check for at least 2 spaces (3 names)
            int spaceCount = name.Count(c => c == ' ');

            // Use InputValidator for name validation
            if (name.Length > 100)
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                FullNameBox.ToolTip = InputValidator.GetValidationMessage("Име", "maxlength");
            }
            else if (spaceCount < 2)
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                FullNameBox.ToolTip = "Моля, въведете три имена (име, презиме, фамилия)";
            }
            else if (!InputValidator.IsValidName(name))
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                FullNameBox.ToolTip = InputValidator.GetValidationMessage("Име", "name");
            }
            else
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                FullNameBox.ToolTip = "Валидно име ✓";
            }
        }

        // Validate Bulgarian EGN (ЕГН)
        private void NationalIdBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string egn = NationalIdBox.Text.Trim();

            if (string.IsNullOrEmpty(egn))
            {
                NationalIdBox.Background = Brushes.White;
                NationalIdBox.ToolTip = null;
                return;
            }

            if (egn.Length != 10)
            {
                NationalIdBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                NationalIdBox.ToolTip = $"ЕГН трябва да е 10 цифри (текущо: {egn.Length})";
            }
            else if (!InputValidator.IsValidEGN(egn))
            {
                NationalIdBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                NationalIdBox.ToolTip = InputValidator.GetValidationMessage("ЕГН", "egn");
            }
            else
            {
                NationalIdBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));

                // Extract birth date from EGN
                DateTime? birthDate = ExtractDateFromEGN(egn);
                if (birthDate.HasValue)
                {
                    NationalIdBox.ToolTip = $"Валиден ЕГН ✓ (Дата на раждане: {birthDate.Value.ToShortDateString()})";

                    // Auto-fill date of birth
                    DOBBox.Text = birthDate.Value.ToString("dd/MM/yyyy");
                }
            }
        }

        // Validate Bulgarian EGN with checksum
        private bool IsValidEGN(string egn)
        {
            if (egn.Length != 10 || !egn.All(char.IsDigit))
                return false;

            // Validate date part
            if (ExtractDateFromEGN(egn) == null)
                return false;

            // Calculate checksum
            int[] weights = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += (egn[i] - '0') * weights[i];
            }

            int remainder = sum % 11;
            int checksum = remainder < 10 ? remainder : 0;

            return checksum == (egn[9] - '0');
        }

        // Extract birth date from EGN
        private DateTime? ExtractDateFromEGN(string egn)
        {
            try
            {
                int year = int.Parse(egn.Substring(0, 2));
                int month = int.Parse(egn.Substring(2, 2));
                int day = int.Parse(egn.Substring(4, 2));

                // Determine century
                if (month > 40)
                {
                    year += 2000;
                    month -= 40;
                }
                else if (month > 20)
                {
                    year += 1800;
                    month -= 20;
                }
                else
                {
                    year += 1900;
                }

                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        // Validate phone number
        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phone = PhoneBox.Text.Trim();

            if (string.IsNullOrEmpty(phone))
            {
                PhoneBox.Background = Brushes.White;
                PhoneBox.ToolTip = null;
                return;
            }

            // Use InputValidator for phone validation
            if (InputValidator.IsValidPhone(phone))
            {
                PhoneBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                PhoneBox.ToolTip = "Валиден телефонен номер ✓";
            }
            else
            {
                PhoneBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                PhoneBox.ToolTip = InputValidator.GetValidationMessage("Телефон", "phone");
            }
        }

        // Validate email
        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = EmailBox.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                EmailBox.Background = Brushes.White;
                EmailBox.ToolTip = null;
                return;
            }

            // Use InputValidator for email validation
            if (email.Length > 100)
            {
                EmailBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                EmailBox.ToolTip = InputValidator.GetValidationMessage("Имейл", "maxlength");
            }
            else if (!InputValidator.IsValidEmail(email))
            {
                EmailBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                EmailBox.ToolTip = InputValidator.GetValidationMessage("Имейл", "email");
            }
            else
            {
                EmailBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                EmailBox.ToolTip = "Валиден имейл ✓";
            }
        }

        // Validate address
        private void AddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string address = AddressBox.Text.Trim();

            if (string.IsNullOrEmpty(address))
            {
                AddressBox.Background = Brushes.White;
                AddressBox.ToolTip = null;
                return;
            }

            // Check address length (minimum 10 characters for a valid address, max 200)
            if (address.Length < 10)
            {
                AddressBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                AddressBox.ToolTip = "Адресът е твърде кратък (мин. 10 символа)";
            }
            else if (address.Length > 200)
            {
                AddressBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                AddressBox.ToolTip = "Адресът е твърде дълъг (макс. 200 символа)";
            }
            else if (!InputValidator.IsValidRequiredText(address, 10, 200))
            {
                AddressBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                AddressBox.ToolTip = "Моля, въведете валиден адрес";
            }
            else
            {
                AddressBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                AddressBox.ToolTip = "Валиден адрес ✓";
            }
        }

        // Validate family consent name (for deceased donors)
        private void FamilyConsentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = FamilyConsentBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                FamilyConsentBox.Background = Brushes.White;
                FamilyConsentBox.ToolTip = null;
                return;
            }

            // Use InputValidator for name validation
            if (!InputValidator.IsValidName(name))
            {
                FamilyConsentBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                FamilyConsentBox.ToolTip = InputValidator.GetValidationMessage("Име на роднина", "name");
            }
            else
            {
                FamilyConsentBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                FamilyConsentBox.ToolTip = "Валидно име ✓";
            }
        }

        // Validate family contact phone (for deceased donors)
        private void FamilyContactPhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phone = FamilyContactPhoneBox.Text.Trim();

            if (string.IsNullOrEmpty(phone))
            {
                FamilyContactPhoneBox.Background = Brushes.White;
                FamilyContactPhoneBox.ToolTip = null;
                return;
            }

            // Use InputValidator for phone validation
            if (InputValidator.IsValidPhone(phone))
            {
                FamilyContactPhoneBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                FamilyContactPhoneBox.ToolTip = "Валиден телефонен номер ✓";
            }
            else
            {
                FamilyContactPhoneBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                FamilyContactPhoneBox.ToolTip = InputValidator.GetValidationMessage("Телефон", "phone");
            }
        }

        // Helper method to parse date from TextBox (dd/MM/yyyy format)
        private DateTime? ParseDateFromTextBox(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
                return null;

            try
            {
                string[] parts = dateText.Split('/');
                if (parts.Length != 3)
                    return null;

                int day = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int year = int.Parse(parts[2]);

                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }

        // Helper method to parse time from TextBox (чч:мм format)
        private (int hour, int minute)? ParseTimeFromTextBox(string timeText)
        {
            if (string.IsNullOrWhiteSpace(timeText))
                return null;

            try
            {
                string[] parts = timeText.Split(':');
                if (parts.Length != 2)
                    return null;

                int hour = int.Parse(parts[0]);
                int minute = int.Parse(parts[1]);

                if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
                    return null;

                return (hour, minute);
            }
            catch
            {
                return null;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Determine donor type
            bool isDeceased = DeceasedDonorRadio?.IsChecked == true;
            string donorType = isDeceased ? "Deceased" : "Living";

            // Collect data from form
            string fullName = FullNameBox.Text.Trim();
            string hospital = MainWindow.CurrentHospital?.Name ?? "";
            DateTime? dob = ParseDateFromTextBox(DOBBox.Text);
            string gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string nationalId = NationalIdBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string address = AddressBox.Text.Trim();

            string bloodType = (BloodTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? BloodTypeBox.Text.Trim();
            string rhFactor = (RhFactorBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? RhFactorBox.Text.Trim();
            string infectious = (InfectiousBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? InfectiousBox.Text.Trim();
            string infectiousOther = InfectiousOtherBox.Text.Trim();

            // Deceased donor specific fields
            DateTime? dateOfDeath = null;
            string causeOfDeath = "";
            string familyConsentGivenBy = "";
            string familyRelationship = "";
            string familyContactPhone = "";

            if (isDeceased)
            {
                // Get date of death with time
                DateTime? deathDate = ParseDateFromTextBox(DateOfDeathBox.Text);
                var deathTime = ParseTimeFromTextBox(DeathTimeBox.Text);

                if (deathDate.HasValue && deathTime.HasValue)
                {
                    dateOfDeath = new DateTime(deathDate.Value.Year, deathDate.Value.Month, deathDate.Value.Day,
                        deathTime.Value.hour, deathTime.Value.minute, 0);
                }

                causeOfDeath = CauseOfDeathBox.Text.Trim();
                familyConsentGivenBy = FamilyConsentBox.Text.Trim();

                // Get family relationship - use "Other" text if selected
                string selectedRelationship = (FamilyRelationshipBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
                if (selectedRelationship == "Друг роднина")
                {
                    familyRelationship = FamilyRelationshipOtherBox.Text.Trim();
                }
                else
                {
                    familyRelationship = selectedRelationship;
                }

                familyContactPhone = FamilyContactPhoneBox.Text.Trim();
            }

            // Additional fields
            string additionalNotes = AdditionalNotesBox.Text.Trim();
            string registeredBy = RegisteredByBox.Text.Trim();

            var selectedOrgans = OrganPanel.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();

            string organsForDonation = string.Join(", ", selectedOrgans);

            // Get organ viability data
            DateTime? harvestDate = ParseDateFromTextBox(HarvestDateBox.Text);
            var harvestTime = ParseTimeFromTextBox(HarvestTimeBox.Text);

            // Get organ quality - use Tag (English) for database
            string organQuality = "";
            if (OrganQualityBox.SelectedItem is ComboBoxItem selectedQuality)
            {
                organQuality = selectedQuality.Tag?.ToString() ?? "";
            }

            // Combine harvest date and time
            DateTime organHarvestTime = DateTime.MinValue;
            if (harvestDate.HasValue && harvestTime.HasValue)
            {
                organHarvestTime = new DateTime(harvestDate.Value.Year, harvestDate.Value.Month, harvestDate.Value.Day,
                    harvestTime.Value.hour, harvestTime.Value.minute, 0);
            }

            // Comprehensive validation
            List<string> errors = new List<string>();

            // Validate full name using InputValidator
            if (string.IsNullOrEmpty(fullName))
            {
                errors.Add("• " + InputValidator.GetValidationMessage("Три имена", "required"));
            }
            else if (fullName.Count(c => c == ' ') < 2)
            {
                errors.Add("• Моля, въведете три имена (име, презиме, фамилия)");
            }
            else if (fullName.Length > 100)
            {
                errors.Add("• " + InputValidator.GetValidationMessage("Име", "maxlength"));
            }
            else if (!InputValidator.IsValidName(fullName))
            {
                errors.Add("• " + InputValidator.GetValidationMessage("Име", "name"));
            }

            // Validate EGN using InputValidator
            if (string.IsNullOrEmpty(nationalId))
            {
                errors.Add("• " + InputValidator.GetValidationMessage("ЕГН", "required"));
            }
            else if (!InputValidator.IsValidEGN(nationalId))
            {
                errors.Add("• " + InputValidator.GetValidationMessage("ЕГН", "egn"));
            }

            // Validate date of birth using InputValidator
            if (string.IsNullOrWhiteSpace(DOBBox.Text))
            {
                errors.Add("• " + InputValidator.GetValidationMessage("Дата на раждане", "required"));
            }
            else if (!dob.HasValue)
            {
                errors.Add("• " + InputValidator.GetValidationMessage("Дата на раждане", "date"));
            }
            else if (!InputValidator.IsValidDateOfBirth(dob.Value))
            {
                errors.Add("• " + InputValidator.GetValidationMessage("Дата на раждане", "date"));
            }

            // Validate EGN date matches DOB
            if (!string.IsNullOrEmpty(nationalId) && nationalId.Length == 10 && dob.HasValue)
            {
                DateTime? egnDate = ExtractDateFromEGN(nationalId);
                if (egnDate.HasValue && egnDate.Value.Date != dob.Value.Date)
                {
                    errors.Add("• Датата на раждане не съвпада с ЕГН");
                }
            }

            // Validate phone using InputValidator
            if (!string.IsNullOrEmpty(phone))
            {
                if (!InputValidator.IsValidPhone(phone))
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Телефон", "phone"));
                }
            }

            // Validate email using InputValidator
            if (!string.IsNullOrEmpty(email))
            {
                if (email.Length > 100)
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Имейл", "maxlength"));
                }
                else if (!InputValidator.IsValidEmail(email))
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Имейл", "email"));
                }
            }

            // Validate address using InputValidator
            if (!string.IsNullOrEmpty(address))
            {
                if (!InputValidator.IsValidRequiredText(address, 10, 200))
                {
                    errors.Add("• Адресът трябва да е между 10 и 200 символа");
                }
            }

            // Validate hospital (from global selection)
            if (string.IsNullOrEmpty(hospital))
            {
                errors.Add("• Моля, изберете болница от главното меню");
            }

            // Validate blood type
            if (string.IsNullOrEmpty(bloodType))
            {
                errors.Add("• Моля, изберете кръвна група");
            }

            // Validate Rh factor
            if (string.IsNullOrEmpty(rhFactor))
            {
                errors.Add("• Моля, изберете резус фактор");
            }

            // Validate at least one organ selected
            if (selectedOrgans.Count == 0)
            {
                errors.Add("• Моля, изберете поне един орган за донорство");
            }

            // Validate deceased donor specific fields
            if (isDeceased)
            {
                if (string.IsNullOrWhiteSpace(DateOfDeathBox.Text))
                {
                    errors.Add("• Дата на смърт е задължително поле");
                }
                else if (!dateOfDeath.HasValue)
                {
                    if (string.IsNullOrWhiteSpace(DeathTimeBox.Text))
                    {
                        errors.Add("• Час на смърт е задължително поле (използвайте чч:мм)");
                    }
                    else
                    {
                        errors.Add("• Невалиден формат на дата/час на смърт");
                    }
                }
                else if (dateOfDeath.Value > DateTime.Now)
                {
                    errors.Add("• Датата на смърт не може да бъде в бъдещето");
                }

                if (string.IsNullOrWhiteSpace(causeOfDeath))
                {
                    errors.Add("• Причина за смърт е задължително поле");
                }

                if (string.IsNullOrWhiteSpace(familyConsentGivenBy))
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Име на роднина", "required"));
                }
                else if (!InputValidator.IsValidName(familyConsentGivenBy))
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Име на роднина", "name"));
                }

                if (FamilyRelationshipBox.SelectedItem == null)
                {
                    errors.Add("• Моля, изберете връзка с починалия");
                }
                else if ((FamilyRelationshipBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Друг роднина" &&
                         string.IsNullOrWhiteSpace(FamilyRelationshipOtherBox.Text))
                {
                    errors.Add("• Моля, опишете връзката с починалия");
                }

                if (string.IsNullOrWhiteSpace(familyContactPhone))
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Телефон на роднина", "required"));
                }
                else if (!InputValidator.IsValidPhone(familyContactPhone))
                {
                    errors.Add("• " + InputValidator.GetValidationMessage("Телефон на роднина", "phone"));
                }

                // Validate organ harvest fields for deceased donors
                if (string.IsNullOrWhiteSpace(HarvestDateBox.Text))
                {
                    errors.Add("• Дата на прибиране на органите е задължително поле");
                }
                else if (organHarvestTime == DateTime.MinValue)
                {
                    if (string.IsNullOrWhiteSpace(HarvestTimeBox.Text))
                    {
                        errors.Add("• Час на прибиране е задължително поле (използвайте чч:мм)");
                    }
                    else
                    {
                        errors.Add("• Невалиден формат на дата/час на прибиране");
                    }
                }
                else if (organHarvestTime > DateTime.Now)
                {
                    errors.Add("• Датата на прибиране не може да бъде в бъдещето");
                }

                if (OrganQualityBox.SelectedItem == null)
                {
                    errors.Add("• Моля, изберете качество на органите");
                }
            }

            // Show errors if any
            if (errors.Count > 0)
            {
                string errorMessage = "Моля, коригирайте следните грешки:\n\n" + string.Join("\n", errors);
                MessageBox.Show(errorMessage, "Грешка при валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save to database using DatabaseHelper
            try
            {
                if (_isEditMode && _editingDonor != null)
                {
                    // Update existing donor
                    _editingDonor.FullName = fullName;
                    _editingDonor.Hospital = hospital;
                    _editingDonor.DateOfBirth = dob.Value;
                    _editingDonor.Gender = gender;
                    _editingDonor.NationalId = nationalId;
                    _editingDonor.Phone = phone;
                    _editingDonor.Email = email;
                    _editingDonor.Address = address;
                    _editingDonor.BloodType = bloodType;
                    _editingDonor.RhFactor = rhFactor;
                    _editingDonor.InfectiousDiseases = infectious;
                    _editingDonor.InfectiousDiseasesOther = infectiousOther;
                    _editingDonor.OrgansForDonation = organsForDonation;
                    _editingDonor.OrganHarvestTime = organHarvestTime;
                    _editingDonor.OrganQuality = organQuality;
                    _editingDonor.DonorType = donorType;
                    _editingDonor.DateOfDeath = dateOfDeath;
                    _editingDonor.CauseOfDeath = causeOfDeath;
                    _editingDonor.FamilyConsentGivenBy = familyConsentGivenBy;
                    _editingDonor.FamilyRelationship = familyRelationship;
                    _editingDonor.FamilyContactPhone = familyContactPhone;
                    _editingDonor.AdditionalNotes = additionalNotes;
                    _editingDonor.RegisteredBy = registeredBy;
                    _editingDonor.UpdatedAt = DateTime.Now;

                    bool success = DatabaseHelper.UpdateDonor(_editingDonor);
                    if (success)
                    {
                        MessageBox.Show("Донорът е обновен успешно!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                }
                else
                {
                    // Create new donor
                    Donor donor = new Donor
                    {
                        FullName = fullName,
                        Hospital = hospital,
                        DateOfBirth = dob.Value,
                        Gender = gender,
                        NationalId = nationalId,
                        Phone = phone,
                        Email = email,
                        Address = address,
                        BloodType = bloodType,
                        RhFactor = rhFactor,
                        InfectiousDiseases = infectious,
                        InfectiousDiseasesOther = infectiousOther,
                        OrgansForDonation = organsForDonation,
                        OrganHarvestTime = organHarvestTime,
                        OrganQuality = organQuality,
                        DonorType = donorType,
                        DateOfDeath = dateOfDeath,
                        CauseOfDeath = causeOfDeath,
                        FamilyConsentGivenBy = familyConsentGivenBy,
                        FamilyRelationship = familyRelationship,
                        FamilyContactPhone = familyContactPhone,
                        AdditionalNotes = additionalNotes,
                        RegisteredBy = registeredBy,
                        CreatedAt = DateTime.Now
                    };

                    bool success = DatabaseHelper.SaveDonor(donor);
                    if (success)
                    {
                        MessageBox.Show("Донорът е регистриран успешно!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при записване: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
