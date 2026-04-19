using Microsoft.Data.SqlClient;
using Models.DTOs;
using System.Data;

namespace Library_Data
{
    public static class clsAuthorData
    {
        public static int AddNewAuthor(clsAuthorDTO authorDTO)
        {
            int? AuthorID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewAuthor", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FirstName", authorDTO.FirstName);
                        command.Parameters.AddWithValue("@LastName", authorDTO.LastName);
                        SqlParameter outputParam = new SqlParameter("@AuthorID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        connection.Open();

                        command.ExecuteNonQuery();
                        AuthorID = (int)command.Parameters["@AuthorID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsAuthorData -> AddNewAuthor: {ex.Message}");
            }
            return AuthorID ?? -1;
        }

        public static bool IsAuthorExists(string FirstName, string LastName)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_IsAuthorExists", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            isFound = reader.HasRows;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsAuthorData -> IsAuthorExists: {ex.Message}");
            }
            return isFound;
        }

        public static clsAuthorDTO FindAuthorByBookID(int BookID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAuthorByBookID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookID", BookID);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsAuthorDTO(
                                    reader.GetInt32(reader.GetOrdinal("AuthorID")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName"))
                                );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsAuthorData -> FindAuthorByBookID: {ex.Message}");
            }
            return null;
        }

        public static clsAuthorDTO Find(string FirstName, string LastName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAuthorByFirstAndLastName", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsAuthorDTO(
                                    reader.GetInt32(reader.GetOrdinal("AuthorID")),
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("LastName"))
                                );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsAuthorData -> Find: {ex.Message}");
            }
            return null;
        }
    }
}
