using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace LittleGymManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LessonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addLesson")]
        public ActionResult<Response> AddLesson(Lesson lesson)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.AddLesson(lesson, connection);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }

            return response;
        }

        [HttpGet]
        [Route("getAllLessons")]
        public ActionResult<List<Lesson>> GetAllLessons()
        {
            List<Lesson> lessons = new List<Lesson>();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    lessons = classDAL.GetAllLessons(connection);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

            return lessons;
        }

        [HttpDelete("/api/Lesson/{id}")]
        public IActionResult DeleteLesson(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL lessonDAL = new DAL();
                    Response response = lessonDAL.DeleteLesson(id, connection);

                    if (response.StatusCode == 200)
                        return Ok(response);
                    else if (response.StatusCode == 404)
                        return NotFound(response);
                    else
                        return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }

        [HttpPut("/api/Lesson/{id}")]
        public IActionResult EditLesson(int id, Lesson lesson)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL lessonDAL = new DAL();
                    lesson.lesson_id = id; // Set the ID from route parameter

                    Response response = lessonDAL.EditLesson(lesson, connection);

                    if (response.StatusCode == 200)
                        return Ok(response);
                    else if (response.StatusCode == 404)
                        return NotFound(response);
                    else
                        return StatusCode(500, response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }

    }
}
