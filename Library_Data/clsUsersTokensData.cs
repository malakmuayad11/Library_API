using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.DTOs;
using System.Data;

namespace Library_Data
{
    public static class clsUsersTokensData
    {
        public static int Login(int UserID, string RefreshTokenHash, DateTime? RefreshTokenExpiresAt)
        {
            int? TokenID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_Login", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@RefreshTokenHash", RefreshTokenHash);
                        command.Parameters.AddWithValue("@RefreshTokenExpiresAt", RefreshTokenExpiresAt);

                        SqlParameter outputParam = new SqlParameter("@TokenID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(outputParam);
                        connection.Open();

                        command.ExecuteNonQuery();
                        TokenID = (int)command.Parameters["@TokenID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                //clsLoggerData.Log($"Error in clUserData -> AddNewUser: {ex}");
                //throw new Exception("DB Error");
            }
            return TokenID ?? -1;
        }

        public static bool Logout(int UserID, DateTime? RefreshTokenRevokedAt)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_Logout", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@RefreshTokenRevokedAt", RefreshTokenRevokedAt ?? (object)DBNull.Value);
                        connection.Open();

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                //clsLoggerData.Log($"Error in clsMemberData -> UpdateMember: {ex.Message}");
                throw new Exception("DB Error");
            }
            return rowsEffected > 0;
        }

        public static string GetRefreshTokenHashForUser(int UserID)
        {
            string RefreshTokenHash = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetRefreshTokenHashForUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result.ToString() != null)
                            RefreshTokenHash = result.ToString();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("DB Error");
                //clsLoggerData.Log($"Error in clsCourseData -> GetNumberOfEnrolledMembers: {ex.Message}");
            }
            return RefreshTokenHash;
        }

        public static DateTime? GetRefreshTokenRevokedAt(int UserID)
        {
            DateTime? RefreshTokenRevokedAt = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetRefreshTokenRevokedAt", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if ((DateTime)result != null)
                            RefreshTokenRevokedAt = (DateTime)result;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("DB Error");
                //clsLoggerData.Log($"Error in clsCourseData -> GetNumberOfEnrolledMembers: {ex.Message}");
            }
            return RefreshTokenRevokedAt;
        }

        public static DateTime? GetRefreshTokenExpiresAt(int UserID)
        {
            DateTime? RefreshTokenExpiresAt = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetRefreshTokenExpiresAt", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if ((DateTime)result != null)
                            RefreshTokenExpiresAt = (DateTime)result;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("DB Error");
                //clsLoggerData.Log($"Error in clsCourseData -> GetNumberOfEnrolledMembers: {ex.Message}");
            }
            return RefreshTokenExpiresAt;
        }

        public static bool Refresh(int UserID, string RefreshTokenHash, DateTime? RefreshTokenExpiresAt)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_Refresh", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@RefreshTokenHash", RefreshTokenHash);
                        command.Parameters.AddWithValue("@RefreshTokenExpiresAt", RefreshTokenExpiresAt ?? (object)DBNull.Value);
                        connection.Open();

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                //clsLoggerData.Log($"Error in clsBookData -> UpdateCondition: {ex.Message}");
                throw new Exception("DB error");
            }
            return rowsEffected > 0;
        }

        public static (DateTime? expiresAt, DateTime? revokedAt,
            string hash) GetTokenDataForUser(int UserID)
        {
            DateTime? expiresAt = null;
            DateTime? revokedAt = null;
            string hash = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                using (SqlCommand command = new SqlCommand("SP_GetTokenDataForUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", UserID);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        { 
                            expiresAt = reader["RefreshTokenExpiresAt"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["RefreshTokenExpiresAt"]);

                            revokedAt = reader["RefreshTokenRevokedAt"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["RefreshTokenRevokedAt"]);

                            hash = reader["RefreshTokenHash"].ToString();
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("DB Error");
            }

            return (expiresAt, revokedAt, hash);
        }
    }
}
