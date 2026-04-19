using Library_Data;
using Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library_Business
{
    public class clsCourse
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

        public enum enMode { AddNew = 1, Update = 2 };
        private enMode _Mode;

        public string TutorName
        {
            get => this.TutorFirstName + " " + this.TutorLastName;
        }

        public string Status
        {
            get
            {
                if (this.EndDate <= DateTime.Now)
                    return "Expired";
                if (this.StartDate > DateTime.Now)
                    return "Comming Soon";
                return "Active";
            }
        }

        public byte NumberOfEnrolledMembers
        {
            get => clsCourseData.GetNumberOfEnrolledMembers(this.CourseID);
        }

        public clsCourseDTO courseDTO
        {
            get => new clsCourseDTO(this.CourseID, this.CourseName, this.TutorFirstName,
                this.TutorLastName, this.EnrollmentFees, this.MaxParticipants, this.StartDate,
                this.EndDate, this.Notes);
        }
        public clsCourse(clsCourseDTO courseDTO, enMode Mode = enMode.AddNew)
        {
            this.CourseID = courseDTO.CourseID;
            this.CourseName = courseDTO.CourseName;
            this.TutorFirstName = courseDTO.TutorFirstName;
            this.TutorLastName = courseDTO.TutorLastName;
            this.EnrollmentFees = courseDTO.EnrollmentFees;
            this.MaxParticipants = courseDTO.MaxParticipants;
            this.StartDate = courseDTO.StartDate;
            this.EndDate = courseDTO.EndDate;
            this.Notes = courseDTO.Notes;
            this._Mode = Mode;
        }

        private bool _AddNewCourse()
        {
            this.CourseID = clsCourseData.AddNewCourse(courseDTO);
            return (this.CourseID != -1);
        }

        private bool _UpdateCourse() => clsCourseData.UpdateCourse(courseDTO);

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCourse())
                    {
                        this._Mode = enMode.Update;
                        return true;
                    }
                    break;
                case enMode.Update:
                    return _UpdateCourse();
            }
            return false;
        }

        public async static Task<List<clsCourseGetAllDTO>> GetAllCoursesAsync() =>
            await clsCourseData.GetAllCoursesAsync();

        public static bool EnrollMemberInCourse(int MemberID, int CourseID) =>
            clsCourseData.EnrollMemberInCourse(MemberID, CourseID);

        public static clsCourse Find(int CourseID)
        {
            clsCourseDTO courseDTO = clsCourseData.Find(CourseID);

            if (courseDTO != null)
                return new clsCourse(courseDTO, enMode.Update);

            return null;
        }

        public async static Task<List<clsMemberGetAllForCourseDTO>> GetAllMembersForCourseAsync(int CourseID)
            => await clsCourseData.GetAllMembersForCourseAsync(CourseID);

        public static bool IsValidInput(clsCourseDTO courseDTO) =>
            !(courseDTO.CourseID < 0 || string.IsNullOrWhiteSpace(courseDTO.CourseName) ||
            string.IsNullOrWhiteSpace(courseDTO.TutorFirstName) || string.IsNullOrWhiteSpace(courseDTO.TutorLastName)
            || courseDTO.EnrollmentFees < 0 || courseDTO.MaxParticipants < 0 ||
            courseDTO.StartDate == null || courseDTO.EndDate == null);
    }
}