using Microsoft.Data.SqlClient;
using Models.DTOs;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public class clsFineData
    {
        public static int AddNewFine(clsFineDTO fineDTO)
        {
            int? FineID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewFine", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", fineDTO.MemberID);
                        command.Parameters.AddWithValue("@LoanID", fineDTO.LoanID);
                        command.Parameters.AddWithValue("@FineAmount", fineDTO.FineAmount);
                        SqlParameter outputParam = new SqlParameter("@NewFineID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        connection.Open();

                        command.ExecuteNonQuery();
                        FineID = (int)command.Parameters["@NewFineID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsFineData -> AddNewFine: {ex.Message}");
            }
            return FineID ?? -1;
        }

        public async static Task<List<clsFineDTO>> GetAllFinesAsync()
        {
            List<clsFineDTO> fines = new List<clsFineDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllFines", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                fines.Add(
                                    new clsFineDTO(
                                        reader.GetInt32(reader.GetOrdinal("FineID")),
                                        reader.GetInt32(reader.GetOrdinal("MemberID")),
                                        reader.GetInt32(reader.GetOrdinal("LoanID")),
                                        reader.GetDecimal(reader.GetOrdinal("FineAmount")),
                                        reader.GetBoolean(reader.GetOrdinal("IsPaid"))
                                    ));

                            }

                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsFineData -> GetAllFines: {ex.Message}");
            }
            return fines;
        }

        public static bool UpdatePaymentStatus(int FineID, bool IsPaid)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdatePaymentStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FineID", FineID);
                        command.Parameters.AddWithValue("@IsPaid", IsPaid);
                        connection.Open();

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsFineData -> GetAllFines: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static bool UpdateFineAmount(int FineID, float FineAmount)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdateFineAmount", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FineID", FineID);
                        command.Parameters.AddWithValue("@FineAmount", FineAmount);
                        connection.Open();

                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsFineData -> GetAllFines: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static decimal? GetMemberUnpaidFines(int MemberID)
        {
            decimal? UnpaidFees = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetMemberUnpaidFines", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", MemberID);
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (decimal.TryParse(result.ToString(), out decimal insertedResult))
                            UnpaidFees = insertedResult;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsFineData -> GetMemberUnpaidFines: {ex.Message}");
            }
            return UnpaidFees ?? -1;
        }
    }
}
