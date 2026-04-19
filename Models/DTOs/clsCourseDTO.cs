namespace Models.DTOs
{
    public class clsCourseDTO
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string TutorFirstName { get; set; }
        public string TutorLastName { get; set; }
        public decimal EnrollmentFees { get; set; }
        public byte MaxParticipants { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }

        public clsCourseDTO(int courseID, string courseName, string tutorFirstName,
            string tutorLastName, decimal enrollmentFees, byte maxParticipants,
            DateTime startDate, DateTime endDate, string? notes)
        {
            CourseID = courseID;
            CourseName = courseName;
            TutorFirstName = tutorFirstName;
            TutorLastName = tutorLastName;
            EnrollmentFees = enrollmentFees;
            MaxParticipants = maxParticipants;
            StartDate = startDate;
            EndDate = endDate;
            Notes = notes;
        }
    }
}
