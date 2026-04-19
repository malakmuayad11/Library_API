using Microsoft.Data.SqlClient;
using Models.DTOs;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public static class clsMembershipTypeData
    {
        public static async Task<List<clsMembershipTypeDTO>> GetAllMembershipTypesAsync()
        {
            List<clsMembershipTypeDTO> list = new List<clsMembershipTypeDTO>();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllMembershipTypes", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                list.Add(
                                    new clsMembershipTypeDTO(
                                      reader.GetInt32(reader.GetOrdinal("MembershipTypeID")),
                                      reader.GetString(reader.GetOrdinal("MembershipName")),
                                      reader.GetByte(reader.GetOrdinal("NumberOfAllowedBooksToBorrow"))
                                    ));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMembershipTypeData -> GetAllMembershipTypesAsync: {ex.Message}");
            }
            return list;
        }

        public static clsMembershipTypeDTO Find(int MembershipTypeID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetMembershipTypeByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MembershipTypeID", MembershipTypeID);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsMembershipTypeDTO(
                                    MembershipTypeID,
                                    reader.GetString(reader.GetOrdinal("MembershipName")),
                                    reader.GetByte(reader.GetOrdinal("NumberOfAllowedBooksToBorrow"))
                               );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsMembershipTypeData -> Find: {ex}");
            }
            return null;
        }
    }
}
