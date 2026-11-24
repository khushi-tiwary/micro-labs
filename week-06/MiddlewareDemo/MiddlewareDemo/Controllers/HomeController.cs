
using Microsoft.AspNetCore.Mvc;

namespace MiddlewareDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        // Action 1: Returns JSON result
        [HttpGet("details")]
        public IActionResult GetDetails()
        {
            var student = new
            {
                Id = 1,
                Name = "Rahul",
                Course = "ASP.NET Core"
            };

            return Ok(student);
        }

        // Action 2: Returns Content result
        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            return Content("Welcome to ASP.NET Core!");
        }

        // Action 3: Returns NotFound result
        [HttpGet("student/{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id != 1)
            {
                return NotFound("Student not found");
            }

            return Ok(new
            {
                Id = 1,
                Name = "Rahul"
            });
        }
    }
}
