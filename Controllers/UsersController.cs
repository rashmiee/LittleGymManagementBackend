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

        [HttpPost]
        [Route("login")]
        public Response login(LoginRequest loginRequest)
        {
            DAL dal = new DAL();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb").ToString());
            Response response = dal.login(loginRequest.Email, loginRequest.Password, connection);
            return response;
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
