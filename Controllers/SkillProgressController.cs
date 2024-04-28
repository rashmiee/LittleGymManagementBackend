using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LittleGymManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillProgressController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SkillProgressController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addSkillProgress")]
        public ActionResult<Response> AddSkillProgress(SkillProgress skillProgress)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.AddSkillProgress(skillProgress, connection);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }

            return response;
        }

    }
}
