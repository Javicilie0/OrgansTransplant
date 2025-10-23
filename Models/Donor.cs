using System;

namespace OrgnTransplant.Models
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
        public string InfectiousDiseases { get; set; }
        public string InfectiousDiseasesOther { get; set; } // For "Other" option details
        public string OrgansForDonation { get; set; }

        // Donor type and deceased donor specific fields
        public string DonorType { get; set; } // "Living" or "Deceased"
        public DateTime? DateOfDeath { get; set; } // Only for deceased donors
        public string CauseOfDeath { get; set; } // Only for deceased donors
        public string FamilyConsentGivenBy { get; set; } // Name of family member who gave consent
        public string FamilyRelationship { get; set; } // Relationship to deceased (e.g., spouse, son, daughter)
        public string FamilyContactPhone { get; set; } // Contact phone of family member
        public string AdditionalNotes { get; set; } // Other notes/information

        // Organ viability fields
        public DateTime OrganHarvestTime { get; set; }
        public string OrganQuality { get; set; } // Excellent, Good, Fair, Poor

        // Tracking fields
        public string RegisteredBy { get; set; } // Name of medical staff who registered the donor
        public DateTime CreatedAt { get; set; } // Timestamp of registration
        public DateTime? UpdatedAt { get; set; } // Timestamp of last update
    }
}
