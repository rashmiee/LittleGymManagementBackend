using System.Data.SqlClient;
using System.Data;

namespace LittleGymManagementBackend.Models
{
    public class DAL
    {
        public Response register(Users users, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                // Check if the user already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @Email", connection);
                checkCmd.Parameters.AddWithValue("@Email", users.Email);

                connection.Open();
                int userCount = (int)checkCmd.ExecuteScalar();
                connection.Close();

                if (userCount > 0)
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "User registration failed. User with this email already exists.";
                    return response;
                }

                // If user doesn't exist, proceed with registration
                SqlCommand cmd = new SqlCommand("sp_register", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FirstName", users.FirstName);
                cmd.Parameters.AddWithValue("@LastName", users.LastName);
                cmd.Parameters.AddWithValue("@Email", users.Email);
                cmd.Parameters.AddWithValue("@PhoneNo", users.PhoneNo);
                cmd.Parameters.AddWithValue("@Password", users.Password);

                // Hardcode Type and Status
                cmd.Parameters.AddWithValue("@Type", "Users");
                cmd.Parameters.AddWithValue("@Status", 1);

                connection.Open();
                int i = cmd.ExecuteNonQuery();
                connection.Close();
                if (i > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "User registered successfully.";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "User registration failed.";
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
                response.StatusCode = 100;
                response.StatusMessage = "User registration failed. Error: " + ex.Message;
            }

            return response;
        }

        public Response login(string email, string password, SqlConnection connection)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("sp_login", connection);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Email", email);
                da.SelectCommand.Parameters.AddWithValue("@Password", password);
                DataTable dt = new DataTable();
                da.Fill(dt);
                Response response = new Response();
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    Users user = new Users
                {
                    ID = Convert.ToInt32(row["ID"]),
                    FirstName = Convert.ToString(row["FirstName"]),
                    LastName = Convert.ToString(row["LastName"]),
                    Email = Convert.ToString(row["Email"]),
                    Type = Convert.ToString(row["Type"])
                };
                    response.StatusCode = 200;
                    response.StatusMessage = "User is valid.";
                    response.user = user;
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "User is invalid.";
                    response.user = null;
                }
                return response;
            }
            catch (Exception ex)
            {
                // Handle the exception for invalid email or password
                Response response = new Response();
                response.StatusCode = 100;
                response.StatusMessage = "User is invalid.";
                response.user = null;
                return response;
            }
        }

        public Response viewUser(int ID, SqlConnection connection)
        {

            SqlDataAdapter da = new SqlDataAdapter("sp_viewUser", connection);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.AddWithValue("@ID", ID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Response response = new Response();
            Users user = new Users();
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                user.ID = row["ID"] == DBNull.Value ? 0 : Convert.ToInt32(row["ID"]); // Assuming ID is of type int
                user.FirstName = row["FirstName"] == DBNull.Value ? string.Empty : Convert.ToString(row["FirstName"]);
                user.LastName = row["LastName"] == DBNull.Value ? string.Empty : Convert.ToString(row["LastName"]);
                user.Email = row["Email"] == DBNull.Value ? string.Empty : Convert.ToString(row["Email"]);
                user.Type = row["Type"] == DBNull.Value ? string.Empty : Convert.ToString(row["Type"]);
                user.CreatedOn = row["CreatedOn"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["CreatedOn"]);
                user.Password = row["Password"] == DBNull.Value ? string.Empty : Convert.ToString(row["Password"]);
                response.StatusCode = 200;
                response.StatusMessage = "User exists.";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "User does not exist.";
                response.user = user;
            }
            return response;
        }

        public Response updateProfile(Users users, SqlConnection connection)
        {

            Response response = new Response();
            SqlCommand cmd = new SqlCommand("sp_updateProfile", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", users.ID);
            cmd.Parameters.AddWithValue("@FirstName", users.FirstName);
            cmd.Parameters.AddWithValue("@LastName", users.LastName);
            cmd.Parameters.AddWithValue("@Email", users.Email);
            cmd.Parameters.AddWithValue("@Password", users.Password);
            connection.Open();
            int success = (int)cmd.ExecuteScalar(); // ExecuteScalar returns the result of the first column of the first row
            connection.Close();

            if (success == 1)
            {
                response.StatusCode = 200;
                response.StatusMessage = "User updated successfully.";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "User update failed.";
            }

            return response;
        }

        public Response userApproval(Users users, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = new SqlCommand("UPDATE Users SET IsApproved = 1 WHERE ID = '" + users.IsApproved + "' AND Status = 1", connection);
            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "User Approved.";
            } else
            {
                response.StatusCode = 100;
                response.StatusMessage = "User Approval failed.";
            }
            return response;
        }

        public Response addNews(News news, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = new SqlCommand("INSERT INTO News(Title,Content,Image,Email,IsActive,CreatedOn VALUES('"+news.Title+"','"+news.Content+ "','"+news.Image+"','" + news.Email+"','1',GETDATE())", connection);
            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "News Created.";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "News Creation failed.";
            }
            return response;
        }

        public Response newsList(SqlConnection connection)
        {
            Response response = new Response();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM News WHERE IsActive = 1;", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<News> lstNews = new List<News>();
            if (dt.Rows.Count > 0)
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    News news = new News();
                    news.ID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    news.Title = Convert.ToString(dt.Rows[i]["Title"]);
                    news.Content = Convert.ToString(dt.Rows[i]["Content"]);
                    news.Image = Convert.ToString(dt.Rows[i]["Image"]);
                    news.Email = Convert.ToString(dt.Rows[i]["Email"]);
                    news.IsActive = Convert.ToInt32(dt.Rows[i]["IsActive"]);
                    news.CreatedOn = Convert.ToDateTime(dt.Rows[i]["CreatedOn"]);
                    lstNews.Add(news);
                }
                if(lstNews.Count > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "News data found.";
                    response.ListNews = lstNews;
                } else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "No News data found.";
                    response.ListNews = null;
                }
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "No News data found.";
                response.ListNews = null;
            }
            return response;
        }

        public Response teachersRegistration(Teachers teacher, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = new SqlCommand("INSERT INTO News(FirstName,LastName,Email,Password,PhoneNo,IsActive VALUES('" + teacher.FirstName + "','" + teacher.LastName + "','" + teacher.Email + "','" + teacher.Password + "','" + teacher.PhoneNo + "','1')", connection);
            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Teacher Registration successfull.";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Teacher Registration failed.";
            }
            return response;
        }

        public Response deleteTeacher(Teachers teacher, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = new SqlCommand("DELETE FROM Teachers WHERE ID = '" + teacher.ID + "' AND Status = 1", connection);
            connection.Open();
            int i = cmd.ExecuteNonQuery();
            connection.Close();

            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Teacher Deletion successfull.";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Teacher Deletion failed.";
            }
            return response;
        }
    }
}
