using Microsoft.Data.SqlClient;
using Models.DTOs;
using System.Data;
using System.Threading.Tasks;

namespace Library_Data
{
    public static class clsCourseData
    {
        public async static Task<List<clsCourseGetAllDTO>> GetAllCoursesAsync()
        {
            List<clsCourseGetAllDTO> courses = new List<clsCourseGetAllDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {

                    using (SqlCommand command = new SqlCommand("SP_GetAllCourses", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                courses.Add(
                                    new clsCourseGetAllDTO(
                                        reader.GetInt32(reader.GetOrdinal("CourseID")),
                                        reader.GetString(reader.GetOrdinal("CourseName")),
                                        reader.GetString(reader.GetOrdinal("TutorName")),
                                        reader.GetDecimal(reader.GetOrdinal("EnrollmentFees")),
                                        reader.GetByte(reader.GetOrdinal("MaxParticipants")),
                                        reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                        reader.GetDateTime(reader.GetOrdinal("EndDate"))
                                        ));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> GetAllCoursesAsync: {ex.Message}");
            }
            return courses;
        }

        public async static Task<List<clsMemberGetAllForCourseDTO>> GetAllMembersForCourseAsync(int CourseID)
        {
            List<clsMemberGetAllForCourseDTO> members = new List<clsMemberGetAllForCourseDTO>();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetAllMembersForCourse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CourseID", CourseID);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while(await reader.ReadAsync())
                            {
                                members.Add(
                                    new clsMemberGetAllForCourseDTO(
                                        reader.GetInt32(reader.GetOrdinal("CourseID")),
                                        reader.GetInt32(reader.GetOrdinal("MemberID")),
                                        reader.GetString(reader.GetOrdinal("FullName")),
                                        reader.GetString(reader.GetOrdinal("Phone")),
                                        reader.GetDateTime(reader.GetOrdinal("EnrollmentDate"))
                                        ));
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> GetAllMembersForCourseAsync: {ex.Message}");
            }
            return members;
        }

        public static int AddNewCourse(clsCourseDTO courseDTO)
        {
            int? CourseID = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_AddNewCourse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CourseName", courseDTO.CourseName);
                        command.Parameters.AddWithValue("@TutorFirstName", courseDTO.TutorFirstName);
                        command.Parameters.AddWithValue("@TutorLastName", courseDTO.TutorLastName);
                        command.Parameters.AddWithValue("@EnrollmentFees", courseDTO.EnrollmentFees);
                        command.Parameters.AddWithValue("@MaxParticipants", courseDTO.MaxParticipants);
                        command.Parameters.AddWithValue("@StartDate", courseDTO.StartDate);
                        command.Parameters.AddWithValue("@EndDate", courseDTO.EndDate);
                        command.Parameters.AddWithValue("@Notes", (object)courseDTO.Notes ?? DBNull.Value);

                        SqlParameter outputParam = new SqlParameter("@NewCourseID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParam);
                        connection.Open();

                        command.ExecuteNonQuery();
                        CourseID = (int)command.Parameters["@NewCourseID"].Value;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> AddNewCourse: {ex.Message}");
            }
            return CourseID ?? -1;
        }

        public static bool EnrollMemberInCourse(int MemberID, int CourseID)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_EnrollMemberInCourse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MemberID", MemberID);
                        command.Parameters.AddWithValue("@CourseID", CourseID);
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out rowsEffected)) { }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> EnrollMemberInCourse: {ex.Message}");
            }
            return rowsEffected > 0;
        }

        public static clsCourseDTO Find(int CourseID)
        { 
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetCourseByID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CourseID", CourseID);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new clsCourseDTO(
                                    CourseID,
                                    reader.GetString(reader.GetOrdinal("CourseName")),
                                    reader.GetString(reader.GetOrdinal("TutorFirstName")),
                                    reader.GetString(reader.GetOrdinal("TutorLastName")),
                                    reader.GetDecimal(reader.GetOrdinal("EnrollmentFees")),
                                    reader.GetByte(reader.GetOrdinal("MaxParticipants")),
                                    reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                    reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                    reader["Notes"] == DBNull.Value ? null : (string)reader["Notes"]
                                    );
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> Find: {ex.Message}");
            }
            return null;
        }

        public static byte GetNumberOfEnrolledMembers(int CourseID)
        {
            byte NumberOfEnrolledMembers = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetNumberOfEnrolledMembers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CourseID", CourseID);
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && byte.TryParse(result.ToString(), out byte insertedResult))
                            NumberOfEnrolledMembers = insertedResult;
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> GetNumberOfEnrolledMembers: {ex.Message}");
            }
            return NumberOfEnrolledMembers;
        }

        public static bool UpdateCourse(clsCourseDTO courseDTO)
        {
            int rowsEffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsSettingsData.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_UpdateCourse", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CourseName", courseDTO.CourseName);
                        command.Parameters.AddWithValue("@TutorFirstName", courseDTO.TutorFirstName);
                        command.Parameters.AddWithValue("@TutorLastName", courseDTO.TutorLastName);
                        command.Parameters.AddWithValue("@EnrollmentFees", courseDTO.EnrollmentFees);
                        command.Parameters.AddWithValue("@MaxParticipants", courseDTO.MaxParticipants);
                        command.Parameters.AddWithValue("@StartDate", courseDTO.StartDate);
                        command.Parameters.AddWithValue("@EndDate", courseDTO.EndDate);
                        command.Parameters.AddWithValue("@Notes", courseDTO.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CourseID", courseDTO.CourseID);

                        connection.Open();
                        rowsEffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                clsLoggerData.Log($"Error in clsCourseData -> UpdateCourse: {ex.Message}");
            }
            return rowsEffected > 0;
        }
    }
}