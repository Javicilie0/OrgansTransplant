using System;

namespace OrgnTransplant
{
    public class Donor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Hospital { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string NationalId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }
        public string RhFactor { get; set; }
        public string Allergies { get; set; }
        public string MedicalConditions { get; set; }
        public string Medications { get; set; }
        public string Surgeries { get; set; }
        public string InfectiousDiseases { get; set; }
        public string OrgansForDonation { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
