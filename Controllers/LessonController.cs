using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
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

    }
}
