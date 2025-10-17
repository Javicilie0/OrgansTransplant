namespace OrgnTransplant
{
    public class OrganInfo
    {
        public string IconPath { get; set; }
        public string OrganName { get; set; }
        public string Hospital { get; set; }
        public string Donor { get; set; }
        public string AdditionalInfo { get; set; }
        public double DistanceKm { get; set; }
        public string DistanceDisplay => DistanceKm < 0
            ? "N/A"
            : (DistanceKm < 1000
                ? $"{DistanceKm:F0} км"
                : $"{DistanceKm:F0} км");

        // Organ viability properties
        public string ViabilityTimeDisplay { get; set; }
        public string ViabilityColor { get; set; }
        public string QualityDisplay { get; set; }
    }
}
