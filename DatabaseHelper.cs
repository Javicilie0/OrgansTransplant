using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OrgnTransplant
{
    public class DatabaseHelper
    {
        private static string connectionString = "Server=shortline.proxy.rlwy.net;Port=12048;Database=railway;User Id=root;Password=sOUKfiykIGLXorMFXWXMFxbhXLBxnmRr;";

        // Get database connection
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Test database connection
        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // Save a new donor to the database
        public static bool SaveDonor(Donor donor)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"INSERT INTO Donors
                        (full_name, dob, gender, national_id, address, phone, email,
                         blood_type, rh_factor, infectious, organs, hospital)
                        VALUES
                        (@full_name, @dob, @gender, @national_id, @address, @phone, @email,
                         @blood_type, @rh_factor, @infectious, @organs, @hospital)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
                        cmd.Parameters.AddWithValue("@organs", donor.OrgansForDonation);
                        cmd.Parameters.AddWithValue("@hospital", donor.Hospital);

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при записване на донор: {ex.Message}");
            }
        }

        // Get all donors from the database
        public static List<Donor> GetAllDonors()
        {
            List<Donor> donors = new List<Donor>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT * FROM Donors";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            donors.Add(new Donor
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
                                OrgansForDonation = reader.IsDBNull(reader.GetOrdinal("organs")) ? "" : reader.GetString("organs")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при четене на донори: {ex.Message}");
            }

            return donors;
        }

        // Get donors by specific organ
        public static List<Donor> GetDonorsByOrgan(string organName)
        {
            List<Donor> donors = new List<Donor>();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
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
                                donors.Add(new Donor
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
                                    OrgansForDonation = reader.IsDBNull(reader.GetOrdinal("organs")) ? "" : reader.GetString("organs")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при търсене на донори: {ex.Message}");
            }

            return donors;
        }

        // Update an existing donor
        public static bool UpdateDonor(Donor donor)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
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
                        organs = @organs
                        WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", donor.Id);
                        cmd.Parameters.AddWithValue("@full_name", donor.FullName);
                        cmd.Parameters.AddWithValue("@hospital", donor.Hospital);
                        cmd.Parameters.AddWithValue("@dob", donor.DateOfBirth);
                        cmd.Parameters.AddWithValue("@gender", donor.Gender);
                        cmd.Parameters.AddWithValue("@national_id", donor.NationalId);
                        cmd.Parameters.AddWithValue("@phone", donor.Phone);
                        cmd.Parameters.AddWithValue("@email", donor.Email);
                        cmd.Parameters.AddWithValue("@address", donor.Address);
                        cmd.Parameters.AddWithValue("@blood_type", donor.BloodType);
                        cmd.Parameters.AddWithValue("@rh_factor", donor.RhFactor);
                        cmd.Parameters.AddWithValue("@infectious", donor.InfectiousDiseases);
                        cmd.Parameters.AddWithValue("@organs", donor.OrgansForDonation);

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при обновяване на донор: {ex.Message}");
            }
        }

        // Delete a donor by ID
        public static bool DeleteDonor(int donorId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
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
                throw new Exception($"Грешка при изтриване на донор: {ex.Message}");
            }
        }
    }
}
