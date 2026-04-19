using Microsoft.Data.SqlClient;
using Models.DTOs;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public class clsMemberData
    {
        public static int AddNewMember(clsMemberDTO memberDTO)
        {
            int? MemberID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewMember", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FirstName", memberDTO.FirstName);
                        command.Parameters.AddWithValue("@SecondName", memberDTO.SecondName);
                        command.Parameters.AddWithValue("@ThirdName", memberDTO.ThirdName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LastName", memberDTO.LastName);
                        command.Parameters.AddWithValue("@DateOfBirth", memberDTO.DateOfBirth);
                        command.Parameters.AddWithValue("@Address", memberDTO.Address);
                        command.Parameters.AddWithValue("@Phone", memberDTO.Phone);
                        command.Parameters.AddWithValue("@Email", memberDTO.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ImagePath", memberDTO.ImagePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MembershipTypeID", memberDTO.MembershipTypeID);

                        SqlParameter outputParam = new SqlParameter("@MemberID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        connection.Open();

                        command.ExecuteNonQuery();
                        MemberID = (int)command.Parameters["@MemberID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> AddNewMember: {ex.Message}");
            }
            return MemberID ?? -1;
        }

        public static bool UpdateMember(clsMemberDTO memberDTO)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_UpdateMember", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FirstName", memberDTO.FirstName);
                        command.Parameters.AddWithValue("@SecondName", memberDTO.SecondName);
                        command.Parameters.AddWithValue("@ThirdName", memberDTO.ThirdName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@LastName", memberDTO.LastName);
                        command.Parameters.AddWithValue("@DateOfBirth", memberDTO.DateOfBirth);
                        command.Parameters.AddWithValue("@Address", memberDTO.Address);
                        command.Parameters.AddWithValue("@Phone", memberDTO.Phone);
                        command.Parameters.AddWithValue("@Email", memberDTO.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ImagePath", memberDTO.ImagePath ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MembershipTypeID", memberDTO.MembershipTypeID);
                        command.Parameters.AddWithValue("@IsCancelled", memberDTO.IsCancelled);
                        command.Parameters.AddWithValue("@MemberID", memberDTO.MemberID);

                        connection.Open();
                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> UpdateMember: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static bool UpdateCancel(int MemberID, bool IsCancelled)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdateCancel", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IsCancelled", IsCancelled);
                        command.Parameters.AddWithValue("@MemberID", MemberID);

                        connection.Open();
                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> UpdateCancel: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static clsMemberDTO Find(int MemberID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetMemberByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", MemberID);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsMemberDTO
                                    (
                                    MemberID,
                                    reader.GetString(reader.GetOrdinal("FirstName")),
                                    reader.GetString(reader.GetOrdinal("SecondName")),
                                    reader["ThirdName"] == DBNull.Value ? null : (string)reader["ThirdName"],
                                    reader.GetString(reader.GetOrdinal("LastName")),
                                    reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                    reader.GetString(reader.GetOrdinal("Address")),
                                    reader.GetString(reader.GetOrdinal("Phone")),
                                    reader["Email"] == DBNull.Value ? null : (string)reader["Email"],
                                    reader["ImagePath"] == DBNull.Value ? null : (string)reader["ImagePath"],
                                    reader.GetInt32(reader.GetOrdinal("MembershipTypeID")),
                                    reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                                    reader.GetBoolean(reader.GetOrdinal("IsCancelled"))
                                    );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> Find: {ex.Message}");
            }
            return null;
        }

        public async static Task<List<clsMemberGetAllDTO>> GetAllMembersAsync()
        {
           List<clsMemberGetAllDTO> members = new List<clsMemberGetAllDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllMembers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                members.Add(
                                    new clsMemberGetAllDTO(
                                    reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    reader.GetString(reader.GetOrdinal("FullName")),
                                    reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                    reader.GetString(reader.GetOrdinal("Phone")),
                                    reader["Email"] == DBNull.Value ? null : (string)reader["Email"],
                                    reader.GetString(reader.GetOrdinal("MembershipName")),
                                    reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                                    reader.GetString(reader.GetOrdinal("Status"))
                                    )
                                );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> GetAllMembers: {ex.Message}");
            }
            return members;
        }

        public static bool RenewMembership(int MemberID)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_RenewMembership", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", MemberID);

                        connection.Open();
                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> RenewMembership: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static int GetNumberOfBorrowedBook(int MemberID)
        {
            int? NumberOfBorrowedBooks = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetNumberOfBorrowedBook", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", MemberID);

                        connection.Open();
                        NumberOfBorrowedBooks = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMemberData -> GetNumberOfBorrowedBook: {ex.Message}");
            }
            return NumberOfBorrowedBooks ?? -1;
        }

    }
}
