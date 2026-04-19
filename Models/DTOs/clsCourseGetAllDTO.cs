namespace Models.DTOs
{
    public class clsCourseGetAllDTO
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string TutorName { get; set; }
        public decimal EnrollmentFees { get; set; }
        public byte MaxParticipants { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public clsCourseGetAllDTO(int courseID, string courseName, string tutorName,
            decimal enrollmentFees, byte maxParticipants, DateTime startDate,
            DateTime endDate)
        {
            CourseID = courseID;
            CourseName = courseName;
            TutorName = tutorName;
            EnrollmentFees = enrollmentFees;
            MaxParticipants = maxParticipants;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
