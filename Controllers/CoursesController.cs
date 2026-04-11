using Library_Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Library_System_API.Controllers
{
    [Route("api/Library/Courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        /// <summary>
        /// Gets all course's info, provided by its id.
        /// </summary>
        /// <param name="MemberID">The ID of the course to find.</param>
        /// <returns>An object full of all course's info.</returns>
        [HttpGet("{CourseID}", Name = "GetCourseByID")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<clsCourseDTO> GetCourseByID(int CourseID)
        {
            if (CourseID < 0)
                return BadRequest("ID is invalid");

            clsCourse course = clsCourse.Find(CourseID);

            if (course != null)
                return Ok(course.courseDTO);

            return NotFound($"Course with id {CourseID} is not found");
        }

        /// <summary>
        /// Adds a new course.
        /// </summary>
        /// <param name="addedCourse">Course's info to be added.</param>
        /// <returns>An object full of all the added course's info if input is valid
        /// and no server error occurs.</returns>
        [HttpPost(Name = "AddNewCourse")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult AddNewCourse(clsCourseDTO addedCourse)
        {
            if (!clsCourse.IsValidInput(addedCourse))
                return BadRequest("Input is invalid");

            clsCourse newCourse = new clsCourse(new clsCourseDTO(addedCourse.CourseID,
                addedCourse.CourseName, addedCourse.TutorFirstName, addedCourse.TutorLastName,
                addedCourse.EnrollmentFees, addedCourse.MaxParticipants, addedCourse.StartDate,
                addedCourse.EndDate, addedCourse.Notes));

            if(!newCourse.Save())
                return StatusCode(StatusCodes.Status500InternalServerError,
             new { message = "An error occurred while adding the new course." });

            addedCourse.CourseID = newCourse.CourseID;

            return CreatedAtRoute("GetCourseByID", new { CourseID = addedCourse.CourseID }, newCourse.courseDTO);

        }

        /// <summary>
        /// Updates all coures's info with provided data. Course ID cannot be updated.
        /// </summary>
        /// <param name="CourseID">The ID of the course that will be updated.</param>
        /// <param name="courseDTO">New info of that course.</param>
        /// <returns>An object full of the updated course's info, if the input
        /// is valid, and no server error occurs.</returns>
        [HttpPut("{CourseID}", Name = "UpdateCourse")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<clsCourseDTO> UpdateCourse(int CourseID, clsCourseDTO courseDTO)
        {
            if (!clsCourse.IsValidInput(courseDTO))
                return BadRequest("Invalid input");

            clsCourse course = clsCourse.Find(CourseID);

            if (course == null)
                return NotFound($"Course with id {CourseID} is not found");

            course.CourseName = courseDTO.CourseName;
            course.TutorFirstName = courseDTO.TutorFirstName;
            course.TutorLastName = courseDTO.TutorLastName;
            course.EnrollmentFees = courseDTO.EnrollmentFees;
            course.MaxParticipants = courseDTO.MaxParticipants;
            course.StartDate = courseDTO.StartDate;
            course.EndDate = courseDTO.EndDate;
            course.Notes = courseDTO.Notes;

            if(!course.Save())
                return StatusCode(500, new { message = "An error occurred while updating the course" });

            return Ok(course.courseDTO);
        }

        /// <summary>
        /// Gets all courses with their approptiate info to display in the presentation layer.
        /// </summary>
        /// <returns>A list of members with their appropriate info.</returns>
        [HttpGet("All", Name = "GetAllCoursesAsync")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsCourseGetAllDTO>>> GetAllCoursesAsync()
        {
            List<clsCourseGetAllDTO> courses = await clsCourse.GetAllCoursesAsync();

            if (courses == null || courses.Count == 0)
                return NotFound("Courses are not found");

            return Ok(courses);
        }

        /// <summary>
        /// Enrolls the specified memeber in the spefied course.
        /// </summary>
        /// <param name="MemberID">The ID of the member to enroll.</param>
        /// <param name="CourseID">The ID of the course to enroll the member in.</param>
        /// <returns>Whether the member is enrolled in the course successfully or not.</returns>
        [HttpGet("EnrollMember/{MemberID}/{CourseID}", Name = "EnrollMemberInCourse")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<bool> EnrollMemberInCourse(int MemberID, int CourseID) =>
            (MemberID < 0 || CourseID < 0) ?
            BadRequest("Input is invalid") :
            Ok(clsCourse.EnrollMemberInCourse(MemberID, CourseID));

        /// <summary>
        /// Gets all enrolled members in the speciefied course.
        /// </summary>
        /// <param name="CourseID">The ID of the course to get its enrolled members.</param>
        /// <returns>A list of members with their appropriate info to be displayed in 
        /// the presentation layer.</returns>
        [HttpGet("MembersForCourse/{CourseID}", Name = "GetAllMembersForCourseAsync")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<clsMemberGetAllForCourseDTO>>> GetAllMembersForCourseAsync(int CourseID)
        {
            if (CourseID < 0)
                return BadRequest("ID is invalid");

            List<clsMemberGetAllForCourseDTO> members = await clsCourse.GetAllMembersForCourseAsync(CourseID);

            if (members.Count == 0 || members == null)
                return NotFound("Members for this course are not found");

            return Ok(members);
        }
    }
}