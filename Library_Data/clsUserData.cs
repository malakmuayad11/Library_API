using Microsoft.Data.SqlClient;
using Models.DTOs;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public static class clsUserData
    {
        public static clsUserDTO Find(int UserID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetUserByID", conn))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        conn.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsUserDTO(
                                    UserID,
                                    reader.GetString(reader.GetOrdinal("Username")),
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetByte(reader.GetOrdinal("Role")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetInt32(reader.GetOrdinal("Permissions"))
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLoggerData.Log($"Error in clsUserData -> Find: {ex.Message}");
            }
            return null;
        }

        public static clsUserDTO Find(string Username)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetUserByUsernameAndPassword", conn))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", Username);
                        conn.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsUserDTO(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    Username,
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetByte(reader.GetOrdinal("Role")),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                    reader.GetInt32(reader.GetOrdinal("Permissions"))
                                    );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLoggerData.Log($"Error in clsUserData -> Find: {ex.Message}");
            }
            return null;
        }

        public static bool UpdatePassword(int UserID, string Password)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdatePassword", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@UserID", UserID);
                        connection.Open();

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsUserData -> UpdatePassword: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        private static byte _GetRole(string Role)
        {
            if (Role == "Admin")
                return 1;
            return 2;
        }

        public async static Task<List<clsUserDTOImportantFields>> GetAllUsersAsync()
        {
            List<clsUserDTOImportantFields> users = new List<clsUserDTOImportantFields>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllUsers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(
                                    new clsUserDTOImportantFields(
                                    reader.GetInt32(reader.GetOrdinal("UserID")),
                                    reader.GetString(reader.GetOrdinal("Username")),
                                    _GetRole(reader.GetString(reader.GetOrdinal("Role"))),
                                    reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                ));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsUserData -> GetAllUsersAsync: {ex.Message}");
            }
            return users;
        }

        public static int AddNewUser(clsUserDTO addedUser)
        {
            int? UserID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", addedUser.Username);
                        command.Parameters.AddWithValue("@Password", addedUser.Password);
                        command.Parameters.AddWithValue("@Role", addedUser.Role);
                        command.Parameters.AddWithValue("@IsActive", addedUser.IsActive);
                        command.Parameters.AddWithValue("@Permissions", addedUser.Permissions);

                        SqlParameter outputParam = new SqlParameter("@UserID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(outputParam);
                        connection.Open();
                        command.ExecuteNonQuery();
                        UserID = (int)command.Parameters["@UserID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clUserData -> AddNewUser: {ex}");
            }
            return UserID ?? -1;
        }

        public static bool DoesUsernameExist(string Username)
        {
            int isFound = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_DoesUsernameExist", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", Username);
                        connection.Open();

                        isFound = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsUserData -> DoesUsernameExist: {ex}");
            }
            return isFound == 1;
        }
    }
}
