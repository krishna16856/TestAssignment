using System.Data;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TestAssignment.Models;
using System.Text;
using System.Security.Cryptography;

namespace TestAssignment.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task RegisterUserAsync(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertUserDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@MobileNo", user.MobileNo);
                    command.Parameters.AddWithValue("@EmailID", user.EmailID);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.PasswordHash); // Password should be hashed before calling this
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@ReportingPersonID", (object)user.ReportingPersonID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReportingPersonRoleID", (object)user.ReportingPersonRoleID ?? DBNull.Value);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                                // Map other fields as needed
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task UpdateUserDetailsAsync(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateUserDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@FullName", (object)user.FullName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MobileNo", (object)user.MobileNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EmailID", (object)user.EmailID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Username", (object)user.Username ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Password", string.IsNullOrEmpty(user.PasswordHash) ? DBNull.Value : ComputeSha256Hash(user.PasswordHash));  // Hash password if provided
                    command.Parameters.AddWithValue("@RoleID", (object)user.RoleID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReportingPersonID", (object)user.ReportingPersonID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ReportingPersonRoleID", (object)user.ReportingPersonRoleID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@isActive", (object)user.isActive ?? DBNull.Value);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256 hash from the rawData string
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                // Return the hexadecimal string as the hash value
                return builder.ToString();
            }
        }
        public async Task DeleteUserAsync(int userID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteUserDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", userID);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<User> GetUserDetailsAsync(int? userID, string username = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetUserDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", (object)userID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Username", (object)username ?? DBNull.Value);

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                                MobileNo = reader.GetString(reader.GetOrdinal("MobileNo")),
                                EmailID = reader.GetString(reader.GetOrdinal("EmailID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                RoleID = reader.GetInt32(reader.GetOrdinal("RoleID")),
                                ReportingPersonID = reader.IsDBNull(reader.GetOrdinal("ReportingPersonID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ReportingPersonID")),
                                ReportingPersonRoleID = reader.IsDBNull(reader.GetOrdinal("ReportingPersonRoleID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ReportingPersonRoleID")),
                                isActive = reader.GetBoolean(reader.GetOrdinal("isActive")),
                                AccountCreatedDateTime = reader.GetDateTime(reader.GetOrdinal("AccountCreatedDateTime")),
                                AccountUpdatedDateTime = reader.GetDateTime(reader.GetOrdinal("AccountUpdatedDateTime"))
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}
