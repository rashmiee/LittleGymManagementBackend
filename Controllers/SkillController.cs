using LittleGymManagementBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LittleGymManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SkillController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addSkill")]
        public ActionResult<Response> AddSkill(Skill skill)
        {
            Response response = new Response();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    response = classDAL.AddSkill(skill, connection);
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
        [Route("getAllSkills")]
        public ActionResult<List<Skill>> GetAllSkills()
        {
            List<Skill> skills = new List<Skill>();
            try
            {
                string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DAL classDAL = new DAL();
                    skills = classDAL.GetAllSkills(connection);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
            return skills;
        }

    }
}
