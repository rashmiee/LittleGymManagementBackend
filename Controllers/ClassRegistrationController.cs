using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LittleGymManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassRegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ClassRegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addClassRegistration")]
        public ActionResult<Response> AddClassRegistration(ClassRegistration registration)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.AddClassRegistration(registration, connection);
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
        [Route("getClassRegistrationsByClassSession/{class_session_id}")]
        public ActionResult<List<ClassRegistration>> GetClassRegistrationsByClassSession(int class_session_id)
        {
            List<ClassRegistration> registrations = new List<ClassRegistration>();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    registrations = classDAL.GetClassRegistrationsByClassSession(class_session_id, connection);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

            return registrations;
        }

        [HttpGet]
        [Route("getClassRegistrationsByUser/{user_id}")]
        public ActionResult<List<ClassRegistration>> GetClassRegistrationsByUser(int user_id)
        {
            List<ClassRegistration> registrations = new List<ClassRegistration>();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    registrations = classDAL.GetClassRegistrationsByUser(user_id, connection);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

            return registrations;
        }

        [HttpPut]
        [Route("updateClassRegistration")]
        public ActionResult<Response> UpdateClassRegistration(ClassRegistration registration)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.UpdateClassRegistration(registration, connection);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }

            return response;
        }

        [HttpDelete]
        [Route("deleteClassRegistration/{registrationID}")]
        public ActionResult<Response> DeleteClassRegistration(int registrationID)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.DeleteClassRegistration(registrationID, connection);
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
        [Route("getAllClassRegistrations")]
        public ActionResult<List<ClassRegistration>> GetAllClassRegistrations()
        {
            List<ClassRegistration> registrations = new List<ClassRegistration>();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    registrations = classDAL.GetAllClassRegistrations(connection);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

            return registrations;
        }
    }
}
