using System;
using System.Text.RegularExpressions;

namespace OrgnTransplant.Utilities
{
    /// <summary>
    /// Input validation helper class
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Validate Bulgarian EGN (Едногражданки Номер)
        /// </summary>
        public static bool IsValidEGN(string egn)
        {
            if (string.IsNullOrWhiteSpace(egn))
                return false;

            // Remove any spaces
            egn = egn.Trim().Replace(" ", "");

            // Must be exactly 10 digits
            if (egn.Length != 10 || !Regex.IsMatch(egn, @"^\d{10}$"))
                return false;

            // Validate checksum
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

        /// <summary>
        /// Validate email address
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Simple regex pattern for email validation
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email.Trim(), pattern);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate Bulgarian phone number
        /// </summary>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove spaces, dashes, and parentheses
            string cleaned = Regex.Replace(phone, @"[\s\-\(\)]", "");

            // Bulgarian phone numbers:
            // Mobile: 08X XXX XXXX (10 digits starting with 08)
            // Landline: 0XX XXX XXX (9 digits starting with 0)
            // International: +359 XX XXX XXXX

            if (cleaned.StartsWith("+359"))
            {
                cleaned = "0" + cleaned.Substring(4);
            }

            // Must start with 0 and be 9-10 digits
            return Regex.IsMatch(cleaned, @"^0\d{8,9}$");
        }

        /// <summary>
        /// Validate required text field
        /// </summary>
        public static bool IsValidRequiredText(string text, int minLength = 1, int maxLength = 1000)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            int length = text.Trim().Length;
            return length >= minLength && length <= maxLength;
        }

        /// <summary>
        /// Validate name (letters, spaces, hyphens only)
        /// </summary>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Allow Cyrillic, Latin letters, spaces, hyphens, and apostrophes
            string pattern = @"^[а-яА-ЯёЁa-zA-Z\s\-']+$";
            return Regex.IsMatch(name.Trim(), pattern) && name.Trim().Length >= 2;
        }

        /// <summary>
        /// Validate date of birth
        /// </summary>
        public static bool IsValidDateOfBirth(DateTime dob)
        {
            // Must be in the past
            if (dob >= DateTime.Now)
                return false;

            // Must be reasonable age (0-120 years)
            int age = DateTime.Now.Year - dob.Year;
            if (dob > DateTime.Now.AddYears(-age))
                age--;

            return age >= 0 && age <= 120;
        }

        /// <summary>
        /// Validate organ harvest time
        /// </summary>
        public static bool IsValidHarvestTime(DateTime harvestTime)
        {
            // Must be in the past or very recent (within 1 hour in future for clock differences)
            return harvestTime <= DateTime.Now.AddHours(1) && harvestTime >= DateTime.Now.AddYears(-1);
        }

        /// <summary>
        /// Get validation error message
        /// </summary>
        public static string GetValidationMessage(string fieldName, string validationType)
        {
            return validationType switch
            {
                "required" => $"{fieldName} е задължително поле.",
                "egn" => "Невалидно ЕГН. Моля, въведете валидно 10-цифрено ЕГН.",
                "email" => "Невалиден имейл адрес. Моля, въведете валиден имейл (example@domain.com).",
                "phone" => "Невалиден телефонен номер. Моля, въведете валиден български телефон (напр. 0888 123 456).",
                "name" => $"{fieldName} трябва да съдържа само букви и да е поне 2 символа.",
                "date" => $"Невалидна дата за {fieldName}.",
                "minlength" => $"{fieldName} е твърде кратко.",
                "maxlength" => $"{fieldName} е твърде дълго.",
                _ => $"Невалидна стойност за {fieldName}."
            };
        }
    }
}
