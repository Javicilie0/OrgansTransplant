
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OrgnTransplant
{
    /// <summary>
    /// Помощен клас за работа със съобщения в базата данни
    /// </summary>
    public static class MessagesHelper
    {
        /// <summary>
        /// Изпраща нова заявка за орган
        /// </summary>
        public static void SendRequest(Message message)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO messages
                        (from_hospital, to_hospital, organ_name, donor_name, donor_id,
                         message_type, delivery_option, status, message_text, created_at)
                        VALUES
                        (@from_hospital, @to_hospital, @organ_name, @donor_name, @donor_id,
                         @message_type, @delivery_option, @status, @message_text, @created_at)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@from_hospital", message.FromHospital);
                        cmd.Parameters.AddWithValue("@to_hospital", message.ToHospital);
                        cmd.Parameters.AddWithValue("@organ_name", message.OrganName);
                        cmd.Parameters.AddWithValue("@donor_name", message.DonorName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@donor_id", message.DonorId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@message_type", message.MessageType.ToString());
                        cmd.Parameters.AddWithValue("@delivery_option", message.DeliveryOption.ToString());
                        cmd.Parameters.AddWithValue("@status", message.Status.ToString());
                        cmd.Parameters.AddWithValue("@message_text", message.MessageText ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@created_at", message.CreatedAt);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при изпращане на заявка: {ex.Message}");
            }
        }

        /// <summary>
        /// Отговаря на заявка
        /// </summary>
        public static void RespondToRequest(int messageId, MessageStatus status, DeliveryOption deliveryOption, string responseText)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE messages
                        SET status = @status,
                            delivery_option = @delivery_option,
                            response_text = @response_text,
                            responded_at = @responded_at
                        WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", messageId);
                        cmd.Parameters.AddWithValue("@status", status.ToString());
                        cmd.Parameters.AddWithValue("@delivery_option", deliveryOption.ToString());
                        cmd.Parameters.AddWithValue("@response_text", responseText ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@responded_at", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при отговор на заявка: {ex.Message}");
            }
        }

        /// <summary>
        /// Взима всички получени заявки за дадена болница (Inbox)
        /// </summary>
        public static List<Message> GetReceivedMessages(string hospitalName)
        {
            List<Message> messages = new List<Message>();

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM messages
                        WHERE to_hospital = @hospital
                        ORDER BY created_at DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hospital", hospitalName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                messages.Add(ReadMessageFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при зареждане на получени съобщения: {ex.Message}");
            }

            return messages;
        }

        /// <summary>
        /// Взима всички изпратени заявки от дадена болница (Outbox)
        /// </summary>
        public static List<Message> GetSentMessages(string hospitalName)
        {
            List<Message> messages = new List<Message>();

            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT * FROM messages
                        WHERE from_hospital = @hospital
                        ORDER BY created_at DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hospital", hospitalName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                messages.Add(ReadMessageFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при зареждане на изпратени съобщения: {ex.Message}");
            }

            return messages;
        }

        /// <summary>
        /// Взима броя на непрочетените заявки (Pending статус)
        /// </summary>
        public static int GetUnreadMessagesCount(string hospitalName)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT COUNT(*) FROM messages
                        WHERE to_hospital = @hospital
                        AND status = 'Pending'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hospital", hospitalName);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при броене на непрочетени съобщения: {ex.Message}");
            }
        }

        /// <summary>
        /// Изтрива съобщение
        /// </summary>
        public static void DeleteMessage(int messageId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM messages WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", messageId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Грешка при изтриване на съобщение: {ex.Message}");
            }
        }

        /// <summary>
        /// Помощен метод за четене на Message обект от MySqlDataReader
        /// </summary>
        private static Message ReadMessageFromReader(MySqlDataReader reader)
        {
            return new Message
            {
                Id = reader.GetInt32("id"),
                FromHospital = reader.GetString("from_hospital"),
                ToHospital = reader.GetString("to_hospital"),
                OrganName = reader.GetString("organ_name"),
                DonorName = reader.IsDBNull(reader.GetOrdinal("donor_name")) ? null : reader.GetString("donor_name"),
                DonorId = reader.IsDBNull(reader.GetOrdinal("donor_id")) ? (int?)null : reader.GetInt32("donor_id"),
                MessageType = (MessageType)Enum.Parse(typeof(MessageType), reader.GetString("message_type")),
                DeliveryOption = (DeliveryOption)Enum.Parse(typeof(DeliveryOption), reader.GetString("delivery_option")),
                Status = (MessageStatus)Enum.Parse(typeof(MessageStatus), reader.GetString("status")),
                MessageText = reader.IsDBNull(reader.GetOrdinal("message_text")) ? null : reader.GetString("message_text"),
                CreatedAt = reader.GetDateTime("created_at"),
                RespondedAt = reader.IsDBNull(reader.GetOrdinal("responded_at")) ? (DateTime?)null : reader.GetDateTime("responded_at"),
                ResponseText = reader.IsDBNull(reader.GetOrdinal("response_text")) ? null : reader.GetString("response_text")
            };
        }
    }
}

