using Microsoft.Data.SqlClient;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public static class clsBookData
    {
        private static clsLoggerData _logger;
        static clsBookData()
        {
            _logger = new clsLoggerData();
        }

        public static int AddNewBook(clsBookDTO bookDTO)
        {
            int? BookID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_AddNewBook", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Title", bookDTO.Title);
                        command.Parameters.AddWithValue("@Genre", bookDTO.Genre);
                        command.Parameters.AddWithValue("@ISBN", bookDTO.ISBN);
                        command.Parameters.AddWithValue("@Condition", bookDTO.Condition);
                        command.Parameters.AddWithValue("@PublicationDate", bookDTO.PublicationDate);
                        command.Parameters.AddWithValue("@Language", bookDTO.Language);
                        SqlParameter outputParam = new SqlParameter("@NewBookID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        command.ExecuteNonQuery();
                        BookID = (int)command.Parameters["@NewBookID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> AddNewBook: {ex.Message}");
            }
            return BookID ?? -1;
        }

        public static bool UpdateCondition(int BookID, byte Condition)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_UpdateCondition", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Condition", Condition);
                        command.Parameters.AddWithValue("@BookID", BookID);

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> UpdateCondition: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static bool UpdateAvailabilityStatus(int BookID, byte AvailabilityStatus)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_UpdateAvailabilityStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AvailabilityStatus", AvailabilityStatus);
                        command.Parameters.AddWithValue("@BookID", BookID);

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> UpdateAvailabilityStatus: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static async Task<List<clsBookGetAllDTO>> GetAllBooksAsync()
        {
            List<clsBookGetAllDTO> books = new List<clsBookGetAllDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetAllBooks", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                books.Add
                                    (
                                    new clsBookGetAllDTO
                                    (
                                        reader.GetInt32(reader.GetOrdinal("BookID")),
                                        reader.GetString(reader.GetOrdinal("Title")),
                                        reader.GetString(reader.GetOrdinal("Genre")),
                                        reader.GetString(reader.GetOrdinal("Condition")),
                                        reader.GetString(reader.GetOrdinal("AvailabilityStatus")),
                                        reader.GetString(reader.GetOrdinal("FullName"))
                                    )
                                );
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> GetAllBooksAsync: {ex.Message}");
            }
            return books;
        }

        public static clsBookDTO Find(int BookID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetBookByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookID", BookID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsBookDTO(
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.GetString(reader.GetOrdinal("Title")),
                                    reader.GetString(reader.GetOrdinal("Genre")),
                                    reader.GetString(reader.GetOrdinal("ISBN")),
                                    reader.GetByte(reader.GetOrdinal("Condition")),
                                    reader.GetDateTime(reader.GetOrdinal("PublicationDate")),
                                    reader.GetByte(reader.GetOrdinal("AvailabilityStatus")),
                                    reader.GetByte(reader.GetOrdinal("Language"))
                                    );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> Find: {ex.Message}");
            }
            return null;
        }

        public static clsBookDTO Find(string Title)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetBookByTitle", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Title", Title);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsBookDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.GetString(reader.GetOrdinal("Title")),
                                    reader.GetString(reader.GetOrdinal("Genre")),
                                    reader.GetString(reader.GetOrdinal("ISBN")),
                                    reader.GetByte(reader.GetOrdinal("Condition")),
                                    reader.GetDateTime(reader.GetOrdinal("PublicationDate")),
                                    reader.GetByte(reader.GetOrdinal("AvailabilityStatus")),
                                    reader.GetByte(reader.GetOrdinal("Language"))
                                );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> Find: {ex.Message}");
            }
            return null;
        }

        public static bool DoesISBNExist(string ISBN)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_DoesISBNExist", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ISBN", ISBN);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            isFound = reader.HasRows;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> DoesISBNExist: {ex.Message}");
            }
            return isFound;
        }

        public static bool AddAuthorToBook(int AuthorID, int BookID)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_AddAuthorToBook", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AuthorID", AuthorID);
                        command.Parameters.AddWithValue("@BookID", BookID);
                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> AddAuthorToBook: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static bool DeleteBook(int BookID)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_DeleteBook", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookID", BookID);

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> DeleteBook: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static int GetAuthorID(int BookID)
        {
            int? AuthorID = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetAuthorID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookID", BookID);
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                            AuthorID = insertedID;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsBookData -> GetAuthorID: {ex.Message}");
            }
            return AuthorID ?? -1;
        }
    }
}