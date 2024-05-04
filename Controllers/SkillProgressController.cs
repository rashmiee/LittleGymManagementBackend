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

        //[HttpPost]
        //[Route("addSkillProgress")]
        //public ActionResult<Response> AddSkillProgress(SkillProgress skillProgress)
        //{
        //    Response response = new Response();
        //    try
        //    {
        //        string connectionString = _configuration.GetConnectionString("LittleGymManagementDb");
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            DAL classDAL = new DAL();
        //            response = classDAL.AddSkillProgress(skillProgress, connection);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = 500;
        //        response.StatusMessage = "An error occurred: " + ex.Message;
        //    }

        //    return response;
        //}

        //[HttpGet("/api/UserSkill/getFinishedSkillsForUser/{userId}")]
        //public IActionResult GetFinishedSkillsForUser(int userId)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("YourConnectionString")))
        //        {
        //            DAL userSkillDAL = new DAL();
        //            // Assuming you have a method in DAL to fetch finished skills for a user
        //            var finishedSkills = userSkillDAL.GetFinishedSkillsForUser(userId, connection);
        //            return Ok(finishedSkills);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
        //    }
        //}

        [HttpGet("/api/SkillProgress/getFinishedSkillsForUser/{userId}")]
        public IActionResult GetFinishedSkillsForUser(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL skillProgressDAL = new DAL();
                    var finishedSkills = skillProgressDAL.GetFinishedSkillsForUser(userId, connection);
                    return Ok(finishedSkills);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }

        //[HttpPost("/api/SkillProgress/addSkillProgress")]
        //public IActionResult AddSkillProgress(SkillProgress skillProgress)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
        //        {
        //            DAL skillProgressDAL = new DAL();
        //            Response response = skillProgressDAL.AddSkillProgress(skillProgress, connection);
        //            return Ok(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
        //    }
        //}

        [HttpGet("/api/SkillProgress/getAllSkillProgress")]
        public IActionResult GetAllSkillProgress()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL skillProgressDAL = new DAL();
                    var allSkillProgress = skillProgressDAL.GetAllSkillProgress(connection);
                    return Ok(allSkillProgress);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }

        [HttpPut("updateSkillProgressStatus")] // Route template for updating skill progress status
        public IActionResult UpdateSkillProgressStatus([FromBody] SkillProgress request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL skillProgressDAL = new DAL();
                    Response response = skillProgressDAL.UpdateSkillProgressStatus(request.User_ID, request.Skill_ID, request.Status, connection);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }

        [HttpPut("/api/SkillProgress/updateUserSkillFeedback")]
        public IActionResult UpdateUserSkillFeedback([FromQuery] int User_ID, [FromQuery] int Skill_ID, [FromQuery] string feedback)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL skillProgressDAL = new DAL();
                    Response response = skillProgressDAL.UpdateUserSkillFeedback(User_ID, Skill_ID, feedback, connection);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }

        [HttpPost("/api/SkillProgress/addSkillProgress")]
        public IActionResult AddSkillProgress([FromQuery] int User_ID, [FromQuery] string Skill_ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("LittleGymManagementDb")))
                {
                    DAL skillProgressDAL = new DAL();
                    Response response = skillProgressDAL.AddSkillProgress(User_ID, Skill_ID, connection);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, StatusMessage = "Internal server error: " + ex.Message });
            }
        }


    }
}
