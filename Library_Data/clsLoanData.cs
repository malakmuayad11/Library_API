using Microsoft.Data.SqlClient;
using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public static class clsLoanData
    {
        private static clsLoggerData _logger;

        static clsLoanData()
        {
            _logger = new clsLoggerData();
        }

        public async static Task<List<clsLoanGetAllDTO>> GetAllLoansAsync()
        {
            List<clsLoanGetAllDTO> loans = new List<clsLoanGetAllDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SP_GetAllLoans", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                loans.Add(
                                    new clsLoanGetAllDTO(
                                        reader.GetInt32(reader.GetOrdinal("LoanID")),
                                        reader.GetString(reader.GetOrdinal("Title")),
                                        reader.GetString(reader.GetOrdinal("FullName")),
                                        reader.GetDateTime(reader.GetOrdinal("LoanStartDate")),
                                        reader.GetDateTime(reader.GetOrdinal("DueDate")),
                                        reader.GetString(reader.GetOrdinal("ReturnDate")),
                                        reader.GetString(reader.GetOrdinal("FineAmount")),
                                        reader.GetString(reader.GetOrdinal("Username"))
                                    )
                                );
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsLoanData -> GetAllLoans: {ex.Message}");
            }
            return loans;
        }

        public static int AddNewLoan(int BookID, int MemberID, int CreatedByUserID)
        {
            int? LoanID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_AddNewLoan", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@BookID", BookID);
                        command.Parameters.AddWithValue("@MemberID", MemberID);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        SqlParameter outputParam = new SqlParameter("@NewLoanID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        command.ExecuteNonQuery();
                        LoanID = (int)command.Parameters["@NewLoanID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            { 
                clsLoggerData.Log($"Error in clsLoanData -> AddNewLoan: {ex.Message}");
            }
            return LoanID ?? -1;
        }

        public static bool ReturnLoan(int LoanID, DateTime ReturnDate, float? FineAmount)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_ReturnLoan", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReturnDate", ReturnDate);
                        command.Parameters.AddWithValue("@FineAmount", FineAmount ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LoanID", LoanID);

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsLoanData -> ReturnLoan: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static clsLoanDTO Find(int LoanID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetLoanByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LoanID", LoanID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsLoanDTO(
                                    reader.GetInt32(reader.GetOrdinal("LoanID")),
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    reader.GetDateTime(reader.GetOrdinal("LoanStartDate")),
                                    reader.GetDateTime(reader.GetOrdinal("DueDate")),
                                    reader["ReturnDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReturnDate"],
                                    reader["FineAmount"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["FineAmount"]),
                                    reader.GetInt32(reader.GetOrdinal("CreatedByUserID"))
                                    );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsLoanData -> Find: {ex.Message}");
            }
            return null;
        }

        public static clsLoanDTO FindByMemberID(int MemberID)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetLoanByMemberID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", MemberID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsLoanDTO(
                                    reader.GetInt32(reader.GetOrdinal("LoanID")),
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    reader.GetDateTime(reader.GetOrdinal("LoanStartDate")),
                                    reader.GetDateTime(reader.GetOrdinal("DueDate")),
                                    reader["ReturnDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ReturnDate"],
                                    reader["FineAmount"] == DBNull.Value ? (float?)null : Convert.ToSingle(reader["FineAmount"]),
                                    reader.GetInt32(reader.GetOrdinal("CreatedByUserID"))
                                    );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsLoanData -> FindByMemberID: {ex.Message}");
            }
            return null;
        }

        public static bool CanReturnBook(int LoanID)
        {
            int isFound = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_CanReturnBook", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@LoanID", LoanID);
                        isFound = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsLoanData -> CanReturnBook: {ex.Message}");
            }
            return isFound == 1;
        }

        public static bool UpdateDueDate(int LoanID, DateTime DueDate)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_UpdateDueDate", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DueDate", DueDate);
                        command.Parameters.AddWithValue("@LoanID", LoanID);

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsLoanData -> UpdateDueDate: {ex.Message}");
            }
            return rowsEffected > 0;
        }
    }
}
