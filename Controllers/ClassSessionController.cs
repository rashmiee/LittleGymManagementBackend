using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace LittleGymManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSessionController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public ClassSessionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addClassSession")]
        public ActionResult<Response> AddClassSession(ClassSession classSession)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.AddClassSession(classSession, connection);
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
        [Route("getClassSessions")]
        public ActionResult<List<ClassSession>> GetClassSessions()
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    List<ClassSession> classSessions = classDAL.GetAllClassSessions(connection);
                    if (classSessions.Count > 0)
                    {
                        return Ok(classSessions);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        response.StatusMessage = "No class sessions found.";
                        return NotFound(response);
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [Route("addClassSessionLesson")]
        public ActionResult<Response> AddLessonToClassSession(int lesson_id, int classSessionId)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.AddLessonToClassSession(lesson_id, classSessionId, connection);
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
        [Route("getClassSessionById/{id}")]
        public ActionResult<ClassSession> GetClassSessionById(int id)
        {
            ClassSession classSession = new ClassSession();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classSessionDAL = new DAL();
                    classSession = classSessionDAL.GetClassSessionById(id, connection);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }

            return classSession;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSession(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL sessionDAL = new DAL();
                    Response response = sessionDAL.DeleteClassSession(id, connection);

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

        [HttpPut("{id}")]
        public IActionResult EditSession(int id, ClassSession session)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL sessionDAL = new DAL();
                    session.SessionClassId = id; // Set the ID from route parameter

                    // Only update allowed fields
                    ClassSession updatedSession = new ClassSession
                    {
                        SessionClassId = session.SessionClassId,
                        Name = session.Name,
                        Category = session.Category,
                        Description = session.Description,
                        Image = session.Image,
                        Price = session.Price,
                        StartTime = session.StartTime,
                        StartDate = session.StartDate,
                        EndDate = session.EndDate
                    };

                    Response response = sessionDAL.EditClassSession(updatedSession, connection);

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
