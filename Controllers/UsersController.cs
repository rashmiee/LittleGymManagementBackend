using LittleGymManagementBackend.Models;
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
    }
}
