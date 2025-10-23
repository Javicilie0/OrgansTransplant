using System;
using OrgnTransplant.Utilities;
using OrgnTransplant.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace OrgnTransplant.Data
{
    public class DonorRepository : IDonorRepository
    {
        private readonly string _connectionString;

        public DonorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Donor>> GetAllDonorsAsync()
        {
            return await Task.Run(() =>
            {
                List<Donor> donors = new List<Donor>();

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Donors";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                donors.Add(MapDonorFromReader(reader));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Failed to get all donors", ex);
                    throw new Exception($"Грешка при четене на донори: {ex.Message}");
                }

                return donors;
            });
        }

        public async Task<List<Donor>> GetDonorsByOrganAsync(string organName)
        {
            return await Task.Run(() =>
            {
                List<Donor> donors = new List<Donor>();

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Donors WHERE organs LIKE @OrganName";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrganName", $"%{organName}%");

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    donors.Add(MapDonorFromReader(reader));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to get donors by organ: {organName}", ex);
                    throw new Exception($"Грешка при търсене на донори: {ex.Message}");
                }

                return donors;
            });
        }

        public async Task<Donor?> GetDonorByIdAsync(int donorId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "SELECT * FROM Donors WHERE id = @id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", donorId);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    return MapDonorFromReader(reader);
                                }
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to get donor by ID: {donorId}", ex);
                    throw new Exception($"Грешка при четене на донор: {ex.Message}");
                }
            });
        }

        public async Task<bool> SaveDonorAsync(Donor donor)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Logger.LogInfo($"Saving new donor: {donor.FullName} (EGN: {donor.NationalId})");
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"INSERT INTO Donors
                            (full_name, dob, gender, national_id, address, phone, email,
                             blood_type, rh_factor, infectious, infectious_other, organs, hospital, organ_harvest_time, organ_quality,
                             donor_type, date_of_death, cause_of_death, family_consent_given_by, family_relationship,
                             family_contact_phone, additional_notes, registered_by, created_at)
                            VALUES
                            (@full_name, @dob, @gender, @national_id, @address, @phone, @email,
                             @blood_type, @rh_factor, @infectious, @infectious_other, @organs, @hospital, @organ_harvest_time, @organ_quality,
                             @donor_type, @date_of_death, @cause_of_death, @family_consent_given_by, @family_relationship,
                             @family_contact_phone, @additional_notes, @registered_by, @created_at)";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            AddDonorParameters(cmd, donor);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Logger.LogInfo($"Donor saved successfully: {donor.FullName}");
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to save donor: {donor.FullName}", ex);
                    throw new Exception($"Грешка при записване на донор: {ex.Message}");
                }
            });
        }

        public async Task<bool> UpdateDonorAsync(Donor donor)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"UPDATE Donors SET
                            full_name = @full_name,
                            hospital = @hospital,
                            dob = @dob,
                            gender = @gender,
                            national_id = @national_id,
                            phone = @phone,
                            email = @email,
                            address = @address,
                            blood_type = @blood_type,
                            rh_factor = @rh_factor,
                            infectious = @infectious,
                            infectious_other = @infectious_other,
                            organs = @organs,
                            organ_harvest_time = @organ_harvest_time,
                            organ_quality = @organ_quality,
                            donor_type = @donor_type,
                            date_of_death = @date_of_death,
                            cause_of_death = @cause_of_death,
                            family_consent_given_by = @family_consent_given_by,
                            family_relationship = @family_relationship,
                            family_contact_phone = @family_contact_phone,
                            additional_notes = @additional_notes,
                            registered_by = @registered_by,
                            updated_at = @updated_at
                            WHERE id = @id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", donor.Id);
                            AddDonorParameters(cmd, donor);
                            cmd.Parameters.AddWithValue("@updated_at", donor.UpdatedAt.HasValue ? (object)donor.UpdatedAt.Value : DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to update donor: {donor.FullName}", ex);
                    throw new Exception($"Грешка при обновяване на донор: {ex.Message}");
                }
            });
        }

        public async Task<bool> DeleteDonorAsync(int donorId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(_connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Donors WHERE id = @id";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", donorId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to delete donor: {donorId}", ex);
                    throw new Exception($"Грешка при изтриване на донор: {ex.Message}");
                }
            });
        }

        private void AddDonorParameters(MySqlCommand cmd, Donor donor)
        {
            cmd.Parameters.AddWithValue("@full_name", donor.FullName);
            cmd.Parameters.AddWithValue("@dob", donor.DateOfBirth);
            cmd.Parameters.AddWithValue("@gender", donor.Gender);
            cmd.Parameters.AddWithValue("@national_id", donor.NationalId);
            cmd.Parameters.AddWithValue("@address", donor.Address);
            cmd.Parameters.AddWithValue("@phone", donor.Phone);
            cmd.Parameters.AddWithValue("@email", donor.Email);
            cmd.Parameters.AddWithValue("@blood_type", donor.BloodType);
            cmd.Parameters.AddWithValue("@rh_factor", donor.RhFactor);
            cmd.Parameters.AddWithValue("@infectious", donor.InfectiousDiseases);
            cmd.Parameters.AddWithValue("@infectious_other", donor.InfectiousDiseasesOther ?? "");
            cmd.Parameters.AddWithValue("@organs", donor.OrgansForDonation);
            cmd.Parameters.AddWithValue("@hospital", donor.Hospital);
            cmd.Parameters.AddWithValue("@organ_harvest_time", donor.OrganHarvestTime);
            cmd.Parameters.AddWithValue("@organ_quality", donor.OrganQuality ?? "");
            cmd.Parameters.AddWithValue("@donor_type", donor.DonorType ?? "Living");
            cmd.Parameters.AddWithValue("@date_of_death", donor.DateOfDeath.HasValue ? (object)donor.DateOfDeath.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@cause_of_death", donor.CauseOfDeath ?? "");
            cmd.Parameters.AddWithValue("@family_consent_given_by", donor.FamilyConsentGivenBy ?? "");
            cmd.Parameters.AddWithValue("@family_relationship", donor.FamilyRelationship ?? "");
            cmd.Parameters.AddWithValue("@family_contact_phone", donor.FamilyContactPhone ?? "");
            cmd.Parameters.AddWithValue("@additional_notes", donor.AdditionalNotes ?? "");
            cmd.Parameters.AddWithValue("@registered_by", donor.RegisteredBy ?? "");
            cmd.Parameters.AddWithValue("@created_at", donor.CreatedAt);
        }

        private Donor MapDonorFromReader(MySqlDataReader reader)
        {
            return new Donor
            {
                Id = reader.GetInt32("id"),
                FullName = reader.GetString("full_name"),
                Hospital = reader.IsDBNull(reader.GetOrdinal("hospital")) ? "" : reader.GetString("hospital"),
                DateOfBirth = reader.GetDateTime("dob"),
                Gender = reader.IsDBNull(reader.GetOrdinal("gender")) ? "" : reader.GetString("gender"),
                NationalId = reader.GetString("national_id"),
                Phone = reader.IsDBNull(reader.GetOrdinal("phone")) ? "" : reader.GetString("phone"),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email"),
                Address = reader.IsDBNull(reader.GetOrdinal("address")) ? "" : reader.GetString("address"),
                BloodType = reader.IsDBNull(reader.GetOrdinal("blood_type")) ? "" : reader.GetString("blood_type"),
                RhFactor = reader.IsDBNull(reader.GetOrdinal("rh_factor")) ? "" : reader.GetString("rh_factor"),
                InfectiousDiseases = reader.IsDBNull(reader.GetOrdinal("infectious")) ? "" : reader.GetString("infectious"),
                InfectiousDiseasesOther = !reader.IsDBNull(reader.GetOrdinal("infectious_other")) ? reader.GetString("infectious_other") : "",
                OrgansForDonation = reader.IsDBNull(reader.GetOrdinal("organs")) ? "" : reader.GetString("organs"),
                OrganHarvestTime = reader.IsDBNull(reader.GetOrdinal("organ_harvest_time")) ? DateTime.MinValue : reader.GetDateTime("organ_harvest_time"),
                OrganQuality = reader.IsDBNull(reader.GetOrdinal("organ_quality")) ? "" : reader.GetString("organ_quality"),
                DonorType = !reader.IsDBNull(reader.GetOrdinal("donor_type")) ? reader.GetString("donor_type") : "Living",
                DateOfDeath = !reader.IsDBNull(reader.GetOrdinal("date_of_death")) ? (DateTime?)reader.GetDateTime("date_of_death") : null,
                CauseOfDeath = !reader.IsDBNull(reader.GetOrdinal("cause_of_death")) ? reader.GetString("cause_of_death") : "",
                FamilyConsentGivenBy = !reader.IsDBNull(reader.GetOrdinal("family_consent_given_by")) ? reader.GetString("family_consent_given_by") : "",
                FamilyRelationship = !reader.IsDBNull(reader.GetOrdinal("family_relationship")) ? reader.GetString("family_relationship") : "",
                FamilyContactPhone = !reader.IsDBNull(reader.GetOrdinal("family_contact_phone")) ? reader.GetString("family_contact_phone") : "",
                AdditionalNotes = !reader.IsDBNull(reader.GetOrdinal("additional_notes")) ? reader.GetString("additional_notes") : "",
                RegisteredBy = !reader.IsDBNull(reader.GetOrdinal("registered_by")) ? reader.GetString("registered_by") : "",
                CreatedAt = !reader.IsDBNull(reader.GetOrdinal("created_at")) ? reader.GetDateTime("created_at") : DateTime.MinValue,
                UpdatedAt = !reader.IsDBNull(reader.GetOrdinal("updated_at")) ? (DateTime?)reader.GetDateTime("updated_at") : null
            };
        }
    }
}
