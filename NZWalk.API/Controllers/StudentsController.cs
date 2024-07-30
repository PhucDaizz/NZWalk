using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalk.API.Controllers
{
    // https:localhost:portnumber/api/students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            string[] studentNames = new string[] { "Dai", "Thay", "Quyet", "Te", "Duong", "Muong", "Mac", "Cuoi", "Qua" };
            return Ok(studentNames);
        }
    }
}
