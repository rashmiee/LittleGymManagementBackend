using System.Data.SqlClient;
using System.Data;

namespace LittleGymManagementBackend.Models
{
    public class DAL
    {
        public Response register(Users users, SqlConnection connection)
        {
            Response response = new Response();
            SqlCommand cmd = new SqlCommand("sp_register", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FirstName", users.FirstName);
            cmd.Parameters.AddWithValue("@LastName", users.LastName);
            cmd.Parameters.AddWithValue("@Email", users.Email);
            cmd.Parameters.AddWithValue("@Password", users.Password);
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

            return response;
        }

        public Response login(string email, string password, SqlConnection connection)
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

    }
}
