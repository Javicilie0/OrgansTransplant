using System;
using System.Collections.Generic;

namespace OrgnTransplant
{
    /// <summary>
    /// Handles organ compatibility checking based on blood type and other factors
    /// </summary>
    public static class OrganCompatibility
    {
        // Blood type compatibility matrix
        // Key: Recipient blood type, Value: Compatible donor blood types
        private static readonly Dictionary<string, List<string>> BloodTypeCompatibility = new Dictionary<string, List<string>>
        {
            { "A", new List<string> { "A", "O" } },
            { "B", new List<string> { "B", "O" } },
            { "AB", new List<string> { "A", "B", "AB", "O" } },  // Universal recipient
            { "O", new List<string> { "O" } }  // Can only receive from O
        };

        // Rh factor compatibility
        // Rh+ can receive from Rh+ or Rh-, but Rh- can only receive from Rh-
        private static readonly Dictionary<string, List<string>> RhCompatibility = new Dictionary<string, List<string>>
        {
            { "+", new List<string> { "+", "-" } },
            { "-", new List<string> { "-" } }
        };

        /// <summary>
        /// Check if donor and recipient are blood type compatible
        /// </summary>
        public static bool IsBloodTypeCompatible(string donorBloodType, string donorRh, string recipientBloodType, string recipientRh)
        {
            // Handle empty or null values
            if (string.IsNullOrWhiteSpace(donorBloodType) || string.IsNullOrWhiteSpace(recipientBloodType))
                return false;

            if (string.IsNullOrWhiteSpace(donorRh) || string.IsNullOrWhiteSpace(recipientRh))
                return false;

            // Check blood type compatibility
            if (!BloodTypeCompatibility.ContainsKey(recipientBloodType))
                return false;

            if (!BloodTypeCompatibility[recipientBloodType].Contains(donorBloodType))
                return false;

            // Check Rh compatibility
            if (!RhCompatibility.ContainsKey(recipientRh))
                return false;

            if (!RhCompatibility[recipientRh].Contains(donorRh))
                return false;

            return true;
        }

        /// <summary>
        /// Get compatibility badge color
        /// </summary>
        public static string GetCompatibilityColor(bool isCompatible)
        {
            return isCompatible ? "#27AE60" : "#E74C3C";
        }

        /// <summary>
        /// Get compatibility badge text
        /// </summary>
        public static string GetCompatibilityText(bool isCompatible)
        {
            return isCompatible ? "✓ Съвместим" : "✗ Несъвместим";
        }

        /// <summary>
        /// Get detailed compatibility description
        /// </summary>
        public static string GetCompatibilityDescription(string donorBloodType, string donorRh, string recipientBloodType, string recipientRh)
        {
            bool compatible = IsBloodTypeCompatible(donorBloodType, donorRh, recipientBloodType, recipientRh);

            if (compatible)
            {
                return $"🟢 Донор {donorBloodType}{donorRh} е съвместим с реципиент {recipientBloodType}{recipientRh}";
            }
            else
            {
                // Provide reason for incompatibility
                if (!BloodTypeCompatibility.ContainsKey(recipientBloodType) ||
                    !BloodTypeCompatibility[recipientBloodType].Contains(donorBloodType))
                {
                    return $"🔴 Несъвместими кръвни групи: {donorBloodType} → {recipientBloodType}";
                }
                else if (!RhCompatibility.ContainsKey(recipientRh) ||
                         !RhCompatibility[recipientRh].Contains(donorRh))
                {
                    return $"🔴 Несъвместим Rh фактор: {donorRh} → {recipientRh}";
                }
                else
                {
                    return $"🔴 Донор {donorBloodType}{donorRh} не е съвместим с реципиент {recipientBloodType}{recipientRh}";
                }
            }
        }

        /// <summary>
        /// Get list of compatible blood types for a recipient
        /// </summary>
        public static List<string> GetCompatibleBloodTypes(string recipientBloodType, string recipientRh)
        {
            List<string> compatible = new List<string>();

            if (!BloodTypeCompatibility.ContainsKey(recipientBloodType))
                return compatible;

            foreach (string bloodType in BloodTypeCompatibility[recipientBloodType])
            {
                foreach (string rh in RhCompatibility[recipientRh])
                {
                    compatible.Add($"{bloodType}{rh}");
                }
            }

            return compatible;
        }

        /// <summary>
        /// Check if organ is viable and compatible
        /// </summary>
        public static bool IsOrganSuitableForTransplant(Donor donor, string organName, string recipientBloodType, string recipientRh)
        {
            // Check blood compatibility
            if (!IsBloodTypeCompatible(donor.BloodType, donor.RhFactor, recipientBloodType, recipientRh))
                return false;

            // Check organ viability
            if (!OrganViability.IsOrganViable(organName, donor.OrganHarvestTime))
                return false;

            // Check for infectious diseases
            if (!string.IsNullOrWhiteSpace(donor.InfectiousDiseases) && donor.InfectiousDiseases != "Няма")
                return false; // High risk

            // Check organ quality
            if (donor.OrganQuality?.ToLower() == "poor")
                return false; // Too risky

            return true;
        }

        /// <summary>
        /// Calculate compatibility score (0-100)
        /// </summary>
        public static int CalculateCompatibilityScore(Donor donor, string organName, string recipientBloodType, string recipientRh)
        {
            int score = 0;

            // Blood type compatibility (40 points)
            if (IsBloodTypeCompatible(donor.BloodType, donor.RhFactor, recipientBloodType, recipientRh))
            {
                score += 40;

                // Perfect match bonus
                if (donor.BloodType == recipientBloodType && donor.RhFactor == recipientRh)
                    score += 10;
            }

            // Organ viability (30 points)
            if (OrganViability.IsOrganViable(organName, donor.OrganHarvestTime))
            {
                TimeSpan remaining = OrganViability.GetRemainingTime(organName, donor.OrganHarvestTime);
                int maxHours = OrganViability.GetViabilityHours(organName);

                if (remaining != TimeSpan.MaxValue && maxHours > 0)
                {
                    double percentRemaining = (remaining.TotalHours / maxHours) * 100;
                    score += (int)(percentRemaining * 0.3); // Up to 30 points
                }
                else
                {
                    score += 30; // No time constraint
                }
            }

            // Organ quality (20 points)
            switch (donor.OrganQuality?.ToLower())
            {
                case "excellent":
                    score += 20;
                    break;
                case "good":
                    score += 15;
                    break;
                case "fair":
                    score += 10;
                    break;
                case "poor":
                    score += 5;
                    break;
                default:
                    score += 10; // Unknown quality
                    break;
            }

            // Infectious diseases penalty (10 points)
            if (string.IsNullOrWhiteSpace(donor.InfectiousDiseases) || donor.InfectiousDiseases == "Няма")
                score += 10;

            return Math.Min(score, 100); // Cap at 100
        }

        /// <summary>
        /// Get compatibility score color
        /// </summary>
        public static string GetScoreColor(int score)
        {
            if (score >= 80)
                return "#27AE60"; // Green - Excellent match
            else if (score >= 60)
                return "#F39C12"; // Orange - Good match
            else if (score >= 40)
                return "#E67E22"; // Dark orange - Fair match
            else
                return "#E74C3C"; // Red - Poor match
        }

        /// <summary>
        /// Get compatibility score description
        /// </summary>
        public static string GetScoreDescription(int score)
        {
            if (score >= 80)
                return "🟢 Отлично съвпадение";
            else if (score >= 60)
                return "🟡 Добро съвпадение";
            else if (score >= 40)
                return "🟠 Задоволително съвпадение";
            else
                return "🔴 Лошо съвпадение";
        }
    }
}
