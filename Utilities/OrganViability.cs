using System;
using System.Collections.Generic;

namespace OrgnTransplant.Utilities
{
    /// <summary>
    /// Defines organ viability timeframes and quality assessment
    /// </summary>
    public static class OrganViability
    {
        // Organ viability hours (maximum time organ can survive outside body)
        private static readonly Dictionary<string, int> OrganLifespanHours = new Dictionary<string, int>
        {
            { "Сърце", 6 },           // Heart: 4-6 hours
            { "Бял дроб", 6 },        // Lung: 4-6 hours
            { "Черен дроб", 24 },     // Liver: 12-24 hours
            { "Бъбрек", 36 },         // Kidney: 24-36 hours
            { "Панкреас", 24 },       // Pancreas: 12-24 hours
            { "Черва", 12 },          // Intestine: 8-12 hours
            { "Стомах", 12 },         // Stomach: 8-12 hours
            { "Артерия", 6 },         // Artery: 4-6 hours
            { "Тимус", 6 }            // Thymus: 4-6 hours
        };

        /// <summary>
        /// Get maximum viability hours for an organ
        /// </summary>
        public static int GetViabilityHours(string organName)
        {
            return OrganLifespanHours.ContainsKey(organName) ? OrganLifespanHours[organName] : 24;
        }

        /// <summary>
        /// Check if organ is still viable
        /// </summary>
        public static bool IsOrganViable(string organName, DateTime harvestTime)
        {
            if (harvestTime == DateTime.MinValue)
                return true; // If no harvest time set, assume viable

            int maxHours = GetViabilityHours(organName);
            TimeSpan elapsed = DateTime.Now - harvestTime;
            return elapsed.TotalHours < maxHours;
        }

        /// <summary>
        /// Get remaining viability time
        /// </summary>
        public static TimeSpan GetRemainingTime(string organName, DateTime harvestTime)
        {
            if (harvestTime == DateTime.MinValue)
                return TimeSpan.MaxValue; // No expiry if not set

            int maxHours = GetViabilityHours(organName);
            DateTime expiryTime = harvestTime.AddHours(maxHours);
            TimeSpan remaining = expiryTime - DateTime.Now;

            return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        }

        /// <summary>
        /// Get viability status color (for UI)
        /// </summary>
        public static string GetViabilityColor(string organName, DateTime harvestTime)
        {
            TimeSpan remaining = GetRemainingTime(organName, harvestTime);

            if (remaining == TimeSpan.MaxValue)
                return "#95A5A6"; // Gray - no time set

            if (remaining == TimeSpan.Zero)
                return "#E74C3C"; // Red - expired

            double remainingHours = remaining.TotalHours;
            int maxHours = GetViabilityHours(organName);
            double percentRemaining = (remainingHours / maxHours) * 100;

            if (percentRemaining > 50)
                return "#27AE60"; // Green - good time
            else if (percentRemaining > 25)
                return "#F39C12"; // Orange - medium time
            else
                return "#E74C3C"; // Red - critical time
        }

        /// <summary>
        /// Format remaining time for display
        /// </summary>
        public static string FormatRemainingTime(string organName, DateTime harvestTime)
        {
            TimeSpan remaining = GetRemainingTime(organName, harvestTime);

            if (remaining == TimeSpan.MaxValue)
                return "Не е зададено";

            if (remaining == TimeSpan.Zero)
                return "Изтекло ⏱️";

            if (remaining.TotalHours >= 24)
                return $"{remaining.Days}д {remaining.Hours}ч";
            else if (remaining.TotalHours >= 1)
                return $"{remaining.Hours}ч {remaining.Minutes}м";
            else
                return $"{remaining.Minutes}м {remaining.Seconds}с";
        }

        /// <summary>
        /// Get quality description
        /// </summary>
        public static string GetQualityDescription(string quality)
        {
            switch (quality?.ToLower())
            {
                case "excellent":
                    return "🟢 Отлично - Перфектно състояние";
                case "good":
                    return "🟡 Добро - Приемливо състояние";
                case "fair":
                    return "🟠 Задоволително - Има рискови фактори";
                case "poor":
                    return "🔴 Лошо - Висок риск";
                default:
                    return "⚪ Неоценено";
            }
        }
    }
}
