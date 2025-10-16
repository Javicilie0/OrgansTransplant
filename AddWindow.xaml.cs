using System;
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

namespace OrgnTransplant
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

            // Add event handlers for real-time validation
            FullNameBox.TextChanged += FullNameBox_TextChanged;
            NationalIdBox.TextChanged += NationalIdBox_TextChanged;
            PhoneBox.TextChanged += PhoneBox_TextChanged;
            EmailBox.TextChanged += EmailBox_TextChanged;
            NationalIdBox.PreviewTextInput += NumericOnly_PreviewTextInput;
            PhoneBox.PreviewTextInput += NumericOnly_PreviewTextInput;

            this.Title = "Регистрация на нов донор";
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
            DOBPicker.SelectedDate = donor.DateOfBirth;
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
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

            // Check length (max 100 characters)
            if (name.Length > 100)
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                FullNameBox.ToolTip = "Твърде дълго име (макс. 100 символа)";
            }
            else if (spaceCount < 2)
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                FullNameBox.ToolTip = "Моля, въведете три имена (име, презиме, фамилия)";
            }
            else if (!Regex.IsMatch(name, @"^[а-яА-Яa-zA-Z\s]+$"))
            {
                FullNameBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                FullNameBox.ToolTip = "Името може да съдържа само букви и интервали";
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
            else if (!IsValidEGN(egn))
            {
                NationalIdBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                NationalIdBox.ToolTip = "Невалиден ЕГН (проверете контролната цифра)";
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
                    DOBPicker.SelectedDate = birthDate.Value;
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

            // Bulgarian phone numbers: 10 digits or with +359
            if (phone.Length < 9 || phone.Length > 13)
            {
                PhoneBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                PhoneBox.ToolTip = "Телефонът трябва да е 10 цифри (напр. 0888123456)";
            }
            else if (phone.Length == 10 && phone.StartsWith("0"))
            {
                PhoneBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                PhoneBox.ToolTip = "Валиден телефонен номер ✓";
            }
            else
            {
                PhoneBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                PhoneBox.ToolTip = "Формат: 0888123456";
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

            // Email regex
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (email.Length > 100)
            {
                EmailBox.Background = new SolidColorBrush(Color.FromRgb(255, 230, 230));
                EmailBox.ToolTip = "Твърде дълъг имейл (макс. 100 символа)";
            }
            else if (!Regex.IsMatch(email, emailPattern))
            {
                EmailBox.Background = new SolidColorBrush(Color.FromRgb(255, 250, 200));
                EmailBox.ToolTip = "Невалиден формат на имейл (напр. example@domain.com)";
            }
            else
            {
                EmailBox.Background = new SolidColorBrush(Color.FromRgb(230, 255, 230));
                EmailBox.ToolTip = "Валиден имейл ✓";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Collect data from form
            string fullName = FullNameBox.Text.Trim();
            string hospital = MainWindow.CurrentHospital?.Name ?? "";
            DateTime? dob = DOBPicker.SelectedDate;
            string gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            string nationalId = NationalIdBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string address = AddressBox.Text.Trim();

            string bloodType = (BloodTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? BloodTypeBox.Text.Trim();
            string rhFactor = (RhFactorBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? RhFactorBox.Text.Trim();
            string allergies = AllergiesBox.Text.Trim();
            string conditions = ConditionsBox.Text.Trim();
            string medications = MedicationsBox.Text.Trim();
            string surgeries = SurgeriesBox.Text.Trim();
            string infectious = (InfectiousBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? InfectiousBox.Text.Trim();

            var selectedOrgans = OrganPanel.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true)
                .Select(cb => cb.Content.ToString())
                .ToList();

            string organsForDonation = string.Join(", ", selectedOrgans);

            // Comprehensive validation
            List<string> errors = new List<string>();

            // Validate full name
            if (string.IsNullOrEmpty(fullName))
            {
                errors.Add("• Три имена е задължително поле");
            }
            else if (fullName.Count(c => c == ' ') < 2)
            {
                errors.Add("• Моля, въведете три имена (име, презиме, фамилия)");
            }
            else if (fullName.Length > 100)
            {
                errors.Add("• Името е твърде дълго (макс. 100 символа)");
            }
            else if (!Regex.IsMatch(fullName, @"^[а-яА-Яa-zA-Z\s]+$"))
            {
                errors.Add("• Името може да съдържа само букви и интервали");
            }

            // Validate EGN
            if (string.IsNullOrEmpty(nationalId))
            {
                errors.Add("• ЕГН е задължително поле");
            }
            else if (nationalId.Length != 10)
            {
                errors.Add("• ЕГН трябва да е точно 10 цифри");
            }
            else if (!IsValidEGN(nationalId))
            {
                errors.Add("• Невалиден ЕГН (проверете контролната цифра или датата)");
            }

            // Validate date of birth
            if (!dob.HasValue)
            {
                errors.Add("• Дата на раждане е задължително поле");
            }
            else if (dob.Value > DateTime.Now)
            {
                errors.Add("• Датата на раждане не може да бъде в бъдещето");
            }
            else if (dob.Value < new DateTime(1900, 1, 1))
            {
                errors.Add("• Невалидна дата на раждане");
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

            // Validate phone
            if (!string.IsNullOrEmpty(phone))
            {
                if (phone.Length != 10 || !phone.StartsWith("0"))
                {
                    errors.Add("• Невалиден телефонен номер (формат: 0888123456)");
                }
            }

            // Validate email
            if (!string.IsNullOrEmpty(email))
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(email, emailPattern))
                {
                    errors.Add("• Невалиден формат на имейл");
                }
                else if (email.Length > 100)
                {
                    errors.Add("• Имейлът е твърде дълъг (макс. 100 символа)");
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
                    _editingDonor.Allergies = allergies;
                    _editingDonor.MedicalConditions = conditions;
                    _editingDonor.Medications = medications;
                    _editingDonor.Surgeries = surgeries;
                    _editingDonor.InfectiousDiseases = infectious;
                    _editingDonor.OrgansForDonation = organsForDonation;

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
                        Allergies = allergies,
                        MedicalConditions = conditions,
                        Medications = medications,
                        Surgeries = surgeries,
                        InfectiousDiseases = infectious,
                        OrgansForDonation = organsForDonation,
                        RegistrationDate = DateTime.Now
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
