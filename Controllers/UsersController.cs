using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace LittleGymManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("registration")]
        public Response register(Users users) 
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());
            
            response = dal.register(users, connection);
            return response;
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string email, string password)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());
            Response response = dal.login(email, password, connection);

            if (response.StatusCode == 200)
            {
                var responseData = new
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage,
                    UserID = response.user?.ID,
                    UserType = response.user?.Type
                };
                return Ok(responseData);
            }
            else
            {
                return BadRequest(new
                {
                    StatusCode = response.StatusCode,
                    StatusMessage = response.StatusMessage
                });
            }
        }

        [HttpPost]
        [Route("viewUser")]
        public Response viewUser(int ID)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());
            Response response = dal.viewUser(ID, connection);
            return response;
        }

        [HttpPost]
        [Route("updateProfile")]
        public Response updateProfile(Users users)
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());
            response = dal.updateProfile(users, connection);
            return response;
        }

        [HttpPost]
        [Route("userApproval")]
        public Response UserApproval(Users users)
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());

            response = dal.userApproval(users, connection);
            return response;
        }

        [HttpPost]
        [Route("teacherRegistration")]
        public Response teachersRegistration(Teachers teacher)
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());

            response = dal.teachersRegistration(teacher, connection);
            return response;
        }

        [HttpPost]
        [Route("deleteTeacher")]
        public Response deleteTeacher(Teachers teacher)
        {
            Response response = new Response();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());

            response = dal.deleteTeacher(teacher, connection);
            return response;
        }

        [HttpGet]
        [Route("teachers")]
        public IActionResult GetTeachers()
        {
            List<Users> teachers = new List<Users>();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());

            // Assuming there's a method in DAL to fetch teachers
            teachers = dal.GetTeachers(connection);

            return Ok(teachers);
        }

        [HttpGet]
        [Route("children")]
        public IActionResult GetChildren(string userEmail)
        {
            List<Users> children = new List<Users>();
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb"));

            // Assuming there's a method in DAL to fetch children based on user email
            children = dal.GetChildren(connection, userEmail);

            return Ok(children);
        }

        [HttpGet]
        [Route("getChildren")]
        public ActionResult<List<Users>> GetChildren()      
        {
            List<Users> children = new List<Users>();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    children = classDAL.GetChildren(connection);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
            return children;
        }

        [HttpPost]
        [Route("forgotPassword")]
        public ActionResult<Response> ForgotPassword(string email, string newPassword)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                DAL dal = new DAL();
                response = dal.ForgotPassword(email, newPassword, connectionString);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }

            return response;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL teacherDAL = new DAL();
                    Response response = teacherDAL.DeleteTeacher(id, connection);

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
        public IActionResult EditUser(int id, Users user)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL userDAL = new DAL();
                    user.ID = id; // Set the ID from route parameter

                    // Only update allowed fields
                    Users updatedUser = new Users
                    {
                        ID = user.ID,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = user.Password
                    };

                    Response response = userDAL.EditUser(updatedUser, connection);

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
