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

                // Check if Type is provided from frontend, if not, default to 'Users'
                string type = !string.IsNullOrEmpty(users.Type) ? users.Type : "Users";
                cmd.Parameters.AddWithValue("@Type", type);

                cmd.Parameters.AddWithValue("@UserEmail", string.IsNullOrEmpty(users.UserEmail) ? (object)DBNull.Value : users.UserEmail);
                // Hardcoding Status to 1 for now, modify if needed
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

        public List<Users> GetTeachers(SqlConnection connection)
        {
            List<Users> teachers = new List<Users>();

            try
            {
                // Open the connection
                connection.Open();

                // Define the SQL query
                string query = "SELECT FirstName, LastName, Email, PhoneNo FROM Users WHERE Type = 'Teachers'";

                // Create a SqlCommand object to execute the query
                SqlCommand command = new SqlCommand(query, connection);

                // Execute the query and get the results
                SqlDataReader reader = command.ExecuteReader();

                // Read the results and add teachers to the list
                while (reader.Read())
                {
                    Users teacher = new Users();
                    teacher.FirstName = reader["FirstName"].ToString();
                    teacher.LastName = reader["LastName"].ToString();
                    teacher.Email = reader["Email"].ToString();
                    teacher.PhoneNo = reader["PhoneNo"].ToString();
                    teachers.Add(teacher);
                }

                // Close the reader
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Close the connection
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return teachers;
        }

        public List<Users> GetChildren(SqlConnection connection, string userEmail)
        {
            List<Users> children = new List<Users>();

            try
            {
                // Open the connection
                connection.Open();

                // Define the SQL query with parameters
                string query = "SELECT FirstName, LastName, Email, PhoneNo FROM Users WHERE Type = 'Child' AND UserEmail = @UserEmail";

                // Create a SqlCommand object to execute the query
                SqlCommand command = new SqlCommand(query, connection);

                // Add parameter for UserEmail
                command.Parameters.AddWithValue("@UserEmail", userEmail);

                // Execute the query and get the results
                SqlDataReader reader = command.ExecuteReader();

                // Read the results and add children to the list
                while (reader.Read())
                {
                    Users child = new Users();
                    child.FirstName = reader["FirstName"].ToString();
                    child.LastName = reader["LastName"].ToString();
                    child.Email = reader["Email"].ToString();
                    child.PhoneNo = reader["PhoneNo"].ToString();
                    children.Add(child);
                }

                // Close the reader
                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Close the connection
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return children;
        }

        public Response AddClassSession(ClassSession classSession, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"INSERT INTO ClassSession (Name, Category, Description, Image, Price, StartTime, StartDate, EndDate) 
                             VALUES (@Name, @Category, @Description, @Image, @Price, @StartTime, @StartDate, @EndDate)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", classSession.Name);
                    cmd.Parameters.AddWithValue("@Category", classSession.Category);
                    cmd.Parameters.AddWithValue("@Description", classSession.Description);
                    cmd.Parameters.AddWithValue("@Image", classSession.Image);
                    cmd.Parameters.AddWithValue("@Price", classSession.Price);
                    cmd.Parameters.AddWithValue("@StartTime", classSession.StartTime);
                    cmd.Parameters.AddWithValue("@StartDate", classSession.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", classSession.EndDate);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Class Created.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Class Creation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public List<ClassSession> GetAllClassSessions(SqlConnection connection)
        {
            List<ClassSession> classSessions = new List<ClassSession>();

            try
            {
                string query = @"
            SELECT 
                cs.*, 
                l.lesson_id AS lesson_id, 
                l.Name AS LessonName, 
                l.Description AS LessonDescription 
            FROM 
                ClassSession cs 
            LEFT JOIN 
                Lesson l ON cs.lesson_id = l.lesson_id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ClassSession classSession = new ClassSession
                        {
                            SessionClassId = (int)reader["SessionClassId"],
                            Name = reader["Name"].ToString(),
                            Category = reader["Category"].ToString(),
                            Description = reader["Description"].ToString(),
                            Image = reader["Image"].ToString(),
                            Price = (decimal)reader["Price"],
                            StartTime = TimeSpan.Parse(Convert.ToString(reader["StartTime"])),
                            StartDate = (DateTime)reader["StartDate"],
                            EndDate = (DateTime)reader["EndDate"],
                            lesson_id = (int)reader["lesson_id"],
                            Lesson = new Lesson
                            {
                                lesson_id = (int)reader["lesson_id"],
                                Name = reader["LessonName"].ToString(),
                                Description = reader["LessonDescription"].ToString()
                            }
                        };
                        classSessions.Add(classSession);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it appropriately
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return classSessions;
        }

        public Response AddLesson(Lesson lesson, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"INSERT INTO Lesson (Name, Description) 
                     VALUES (@Name, @Description)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", lesson.Name);
                    cmd.Parameters.AddWithValue("@Description", lesson.Description);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Lesson Created.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Lesson Creation failed.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }

            return response;
        }

        public List<Lesson> GetAllLessons(SqlConnection connection)
        {
            List<Lesson> lessons = new List<Lesson>();
            try
            {
                string query = "SELECT * FROM Lesson";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Lesson lesson = new Lesson
                        {
                            lesson_id = Convert.ToInt32(reader["lesson_id"]),
                            Name = reader["Name"].ToString(),
                            Description = reader["Description"].ToString()
                        };
                        lessons.Add(lesson);
                    }

                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw ex;
            }

            return lessons;
        }

        public Response AddLessonToClassSession(int lesson_id, int classSessionId, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"UPDATE ClassSession SET lesson_id = @lesson_id WHERE SessionClassId = @ClassSessionId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@lesson_id", lesson_id);
                    cmd.Parameters.AddWithValue("@ClassSessionId", classSessionId);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Lesson added to class session.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Failed to add lesson to class session.";
                    }
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
