namespace Models.DTOs
{
    public class clsMemberGetAllForCourseDTO
    {
        public int CourseID { get; set; }
        public int MemberID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public clsMemberGetAllForCourseDTO(int courseID, int memberID, string fullName,
            string phone, DateTime enrollmentDate)
        {
            CourseID = courseID;
            MemberID = memberID;
            FullName = fullName;
            Phone = phone;
            EnrollmentDate = enrollmentDate;
        }
    }
}
