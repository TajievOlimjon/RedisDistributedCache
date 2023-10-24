using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(
            IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents([FromQuery] StudentFilter filter, CancellationToken cancellationToken = default)
        {
            var students = await _studentService.GetAllStudentsAsync(filter, cancellationToken);
            if (students != null)
            {
                return StatusCode((int)HttpStatusCode.OK, students);
            }
            return StatusCode((int)HttpStatusCode.NoContent, students);
        }
        [HttpGet("GetStudentById")]
        public async Task<IActionResult> GetStudentById([FromQuery] int studentId, CancellationToken cancellationToken=default)
        {
            var student = await _studentService.GetStudentByIdAsync(studentId, cancellationToken);
            return StatusCode(student.StatusCode, student);
        }
        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentDto model, CancellationToken cancellationToken = default)
        {
            var response = await _studentService.AddStudentAsync(model, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentDto model, CancellationToken cancellationToken = default)
        {
            var response = await _studentService.UpdateStudentAsync(model, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
        [HttpDelete("DeleteStudent")]
        public async Task<IActionResult> DeleteStudent([FromQuery] int studentId,CancellationToken cancellationToken = default)
        {
            var response = await _studentService.DeleteStudentAsync(studentId,cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
