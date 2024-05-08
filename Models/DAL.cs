using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

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
                string query = "SELECT ID, FirstName, LastName, Email, PhoneNo FROM Users WHERE Type = 'Teachers'";

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
                    teacher.ID = Convert.ToInt32(reader["ID"]);
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
                string query = "SELECT ID, FirstName, LastName, Email, PhoneNo FROM Users WHERE Type = 'Child' AND UserEmail = @UserEmail";

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
                    child.ID = Convert.ToInt32(reader["ID"]);
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
                            lesson_id = DBNull.Value.Equals(reader["lesson_id"]) ? 0 : (int)reader["lesson_id"], // Handle DBNull
                            Lesson = new Lesson
                            {
                                lesson_id = DBNull.Value.Equals(reader["lesson_id"]) ? 0 : (int)reader["lesson_id"], // Handle DBNull
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

        public Response AddSkill(Skill skill, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"INSERT INTO Skills (Name, Description) 
                         VALUES (@Name, @Description)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", skill.Name);
                    cmd.Parameters.AddWithValue("@Description", skill.Description);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Skill Created.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Skill Creation failed.";
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

        //public Response AddSkillProgress(SkillProgress skillProgress, SqlConnection connection)
        //{
        //    Response response = new Response();
        //    try
        //    {
        //        string query = @"INSERT INTO skill_progress (user_ID, skill_ID, status, feedback) 
        //                 VALUES (@User_ID, @Skill_ID, @Status, @Feedback)";

        //        using (SqlCommand cmd = new SqlCommand(query, connection))
        //        {
        //            cmd.Parameters.AddWithValue("@User_ID", skillProgress.User_ID);
        //            cmd.Parameters.AddWithValue("@Skill_ID", skillProgress.Skill_ID);
        //            cmd.Parameters.AddWithValue("@Status", skillProgress.Status);
        //            cmd.Parameters.AddWithValue("@Feedback", skillProgress.Feedback);

        //            connection.Open();
        //            int rowsAffected = cmd.ExecuteNonQuery();
        //            connection.Close();

        //            if (rowsAffected > 0)
        //            {
        //                response.StatusCode = 200;
        //                response.StatusMessage = "Skill Progress Added.";
        //            }
        //            else
        //            {
        //                response.StatusCode = 100;
        //                response.StatusMessage = "Skill Progress Addition failed.";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = 500;
        //        response.StatusMessage = "An error occurred: " + ex.Message;
        //    }

        //    return response;
        //}

        public List<Skill> GetAllSkills(SqlConnection connection)
        {
            List<Skill> skills = new List<Skill>();
            try
            {
                string query = @"SELECT Skill_ID, Name, Description FROM Skills";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Skill skill = new Skill
                            {
                                Skill_ID = Convert.ToInt32(reader["Skill_ID"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString()
                            };
                            skills.Add(skill);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return skills;
        }

        public List<Users> GetChildren(SqlConnection connection)
        {
            List<Users> children = new List<Users>();
            try
            {
                string query = @"SELECT ID, FirstName, LastName, Email, Type, Status, CreatedOn, IsApproved, PhoneNo, UserEmail 
                         FROM Users
                         WHERE Type = 'Child'";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Users child = new Users
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Type = reader["Type"].ToString(),
                                PhoneNo = reader["PhoneNo"].ToString(),
                                UserEmail = reader["UserEmail"].ToString()
                            };
                            children.Add(child);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return children;
        }

        public Response ForgotPassword(string email, string newPassword, string connectionString)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                string query = @"UPDATE Users SET Password = @NewPassword WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);
                    cmd.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Password reset successfully.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Failed to reset password. Email not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = "An error occurred: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response DeleteTeacher(int teacherId, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE ID = @TeacherId", connection);
                cmd.Parameters.AddWithValue("@TeacherId", teacherId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Teacher deleted successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Teacher not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error deleting teacher: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response EditUser(Users user, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNo = @PhoneNo WHERE ID = @UserID", connection);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);
                cmd.Parameters.AddWithValue("@UserID", user.ID);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "User information updated successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "User not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error updating user information: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public ClassSession GetClassSessionById(int id, SqlConnection connection)
        {
            ClassSession classSession = new ClassSession();
            try
            {
                string query = @"SELECT * FROM ClassSession WHERE SessionClassId = @Id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            classSession.SessionClassId = reader.GetInt32(reader.GetOrdinal("SessionClassId"));
                            classSession.Name = reader.GetString(reader.GetOrdinal("Name"));
                            classSession.Category = reader.GetString(reader.GetOrdinal("Category"));
                            classSession.Description = reader.GetString(reader.GetOrdinal("Description"));
                            classSession.Image = reader.GetString(reader.GetOrdinal("Image"));
                            classSession.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                            classSession.StartTime = TimeSpan.Parse(Convert.ToString(reader["StartTime"]));
                            classSession.StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                            classSession.EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                        }
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                // Handle exceptions
            }

            return classSession;
        }

        public Response EditClassSession(ClassSession session, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE ClassSession SET Name = @Name, Category = @Category, Description = @Description, Image = @Image, Price = @Price, StartTime = @StartTime, StartDate = @StartDate, EndDate = @EndDate WHERE SessionClassId = @SessionClassId", connection);
                cmd.Parameters.AddWithValue("@Name", session.Name);
                cmd.Parameters.AddWithValue("@Category", session.Category);
                cmd.Parameters.AddWithValue("@Description", session.Description);
                cmd.Parameters.AddWithValue("@Image", session.Image ?? (object)DBNull.Value); // handle nullable Image
                cmd.Parameters.AddWithValue("@Price", session.Price);
                cmd.Parameters.AddWithValue("@StartTime", session.StartTime);
                cmd.Parameters.AddWithValue("@StartDate", session.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", session.EndDate);
                cmd.Parameters.AddWithValue("@SessionClassId", session.SessionClassId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Session information updated successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Session not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error updating session information: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response DeleteClassSession(int sessionId, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM ClassSession WHERE SessionClassId = @SessionId", connection);
                cmd.Parameters.AddWithValue("@SessionId", sessionId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Session deleted successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Session not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error deleting session: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response DeleteLesson(int lessonId, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Lesson WHERE lesson_id = @LessonId", connection);
                cmd.Parameters.AddWithValue("@LessonId", lessonId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Lesson deleted successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Lesson not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error deleting lesson: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response EditLesson(Lesson lesson, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE Lesson SET Name = @Name, Description = @Description WHERE lesson_id = @LessonId", connection);
                cmd.Parameters.AddWithValue("@Name", lesson.Name);
                cmd.Parameters.AddWithValue("@Description", lesson.Description);
                cmd.Parameters.AddWithValue("@LessonId", lesson.lesson_id);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Lesson information updated successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Lesson not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error updating lesson information: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response DeleteSkill(int skillId, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Skills WHERE skill_id = @SkillId", connection);
                cmd.Parameters.AddWithValue("@SkillId", skillId);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Skill deleted successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Skill not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error deleting skill: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response EditSkill(Skill skill, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE Skills SET Name = @Name, Description = @Description WHERE skill_id = @SkillId", connection);
                cmd.Parameters.AddWithValue("@Name", skill.Name);
                cmd.Parameters.AddWithValue("@Description", skill.Description);
                cmd.Parameters.AddWithValue("@SkillId", skill.Skill_ID);

                connection.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Skill information updated successfully.";
                }
                else
                {
                    response.StatusCode = 404; // Assuming 404 for not found
                    response.StatusMessage = "Skill not found.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error updating skill information: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public List<SkillProgressData> GetFinishedSkillsForUser(int userId, SqlConnection connection)
        {
            List<SkillProgressData> finishedSkills = new List<SkillProgressData>();

            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT s.Skill_ID, s.Name, s.Description, sp.status, sp.feedback 
                                           FROM Skills s 
                                           INNER JOIN skill_progress sp ON s.Skill_ID = sp.skill_id 
                                           WHERE sp.user_id = @UserId AND sp.status = 'Completed'", connection);
                cmd.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    SkillProgressData skillProgressData = new SkillProgressData
                    {
                        Skill_ID = Convert.ToInt32(reader["Skill_ID"]),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["status"].ToString(),
                        Feedback = reader["feedback"].ToString()
                    };
                    finishedSkills.Add(skillProgressData);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return finishedSkills;
        }

        //public Response AddSkillProgress(SkillProgress skillProgress, SqlConnection connection)
        //{
        //    Response response = new Response();

        //    try
        //    {
        //        string query = @"INSERT INTO SkillProgress (user_id, skill_id, status, feedback) 
        //         VALUES (@UserId, @SkillId, @Status, @Feedback)";

        //        using (SqlCommand cmd = new SqlCommand(query, connection))
        //        {
        //            cmd.Parameters.AddWithValue("@UserId", skillProgress.User_ID);
        //            cmd.Parameters.AddWithValue("@SkillId", skillProgress.Skill_ID);
        //            cmd.Parameters.AddWithValue("@Status", skillProgress.Status);
        //            cmd.Parameters.AddWithValue("@Feedback", skillProgress.Feedback);

        //            connection.Open();
        //            int rowsAffected = cmd.ExecuteNonQuery();
        //            connection.Close();

        //            if (rowsAffected > 0)
        //            {
        //                response.StatusCode = 200;
        //                response.StatusMessage = "Skill Progress Added.";
        //            }
        //            else
        //            {
        //                response.StatusCode = 100;
        //                response.StatusMessage = "Skill Progress Addition failed.";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = 500;
        //        response.StatusMessage = "An error occurred: " + ex.Message;
        //    }

        //    return response;
        //}

        public List<SkillProgress> GetAllSkillProgress(SqlConnection connection)
        {
            List<SkillProgress> allSkillProgress = new List<SkillProgress>();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM skill_progress", connection);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    SkillProgress progress = new SkillProgress
                    {
                        Progress_ID = Convert.ToInt32(reader["progress_id"]),
                        User_ID = Convert.ToInt32(reader["user_id"]),
                        Skill_ID = Convert.ToInt32(reader["skill_id"]),
                        Status = reader["status"].ToString(),
                        Feedback = reader["feedback"].ToString()
                    };
                    allSkillProgress.Add(progress);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return allSkillProgress;
        }

        public Response UpdateSkillProgressStatus(int userId, int skillId, string status, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                string query = @"UPDATE skill_progress SET status = @Status WHERE user_id = @UserId AND skill_id = @SkillId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@SkillId", skillId);
                    cmd.Parameters.AddWithValue("@Status", status);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Skill progress status updated successfully.";
                    }
                    else
                    {
                        response.StatusCode = 404; // Assuming 404 for not found
                        response.StatusMessage = "Skill progress not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error updating skill progress status: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response UpdateUserSkillFeedback(int userId, int skillId, string feedback, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                string query = @"UPDATE skill_progress SET feedback = @Feedback WHERE user_ID = @UserId AND skill_ID = @SkillId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@SkillId", skillId);
                    cmd.Parameters.AddWithValue("@Feedback", feedback);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "User skill feedback updated successfully.";
                    }
                    else
                    {
                        response.StatusCode = 404; // Assuming 404 for not found
                        response.StatusMessage = "User skill not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal server error
                response.StatusMessage = "Error updating user skill feedback: " + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return response;
        }

        public Response AddSkillProgress(int userId, string skillId, SqlConnection connection)
        {
            Response response = new Response();

            try
            {
                string query = @"INSERT INTO skill_progress (user_ID, skill_ID) VALUES (@UserId, @SkillId)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@SkillId", skillId);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Skill progress added successfully.";
                    }
                    else
                    {
                        response.StatusCode = 100; // Adjust as needed
                        response.StatusMessage = "Skill progress addition failed.";
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

        public Response AddClassRegistration(ClassRegistration classregistration, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"INSERT INTO ClassRegistration (user_id, class_session_id, payment, register_date) 
                             VALUES (@user_id, @class_session_id, @Payment, @register_date)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    DateTime currentDate = DateTime.Now;
                    string formattedDate = currentDate.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    cmd.Parameters.AddWithValue("@user_id", classregistration.User_id);
                    cmd.Parameters.AddWithValue("@class_session_id", classregistration.Class_session_id);
                    cmd.Parameters.AddWithValue("@Payment", classregistration.Payment);
                    cmd.Parameters.AddWithValue("@register_date", formattedDate);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Class registration created.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Class registration creation failed.";
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

        public List<ClassRegistration> GetClassRegistrationsByClassSession(int class_session_id, SqlConnection connection)
        {
            List<ClassRegistration> registrations = new List<ClassRegistration>();
            try
            {
                string query = @"SELECT * FROM ClassRegistration WHERE class_session_id = @class_session_id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@class_session_id", class_session_id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClassRegistration registration = new ClassRegistration
                            {
                                Registration_id = Convert.ToInt32(reader["registration_id"]),
                                User_id = Convert.ToInt32(reader["user_id"]),
                                Class_session_id = Convert.ToInt32(reader["class_session_id"]),
                                Payment = Convert.ToBoolean(reader["payment"]),
                                Register_date = Convert.ToDateTime(reader["register_date"])
                            };
                            registrations.Add(registration);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                throw ex;
            }

            return registrations;
        }

        public List<ClassRegistration> GetClassRegistrationsByUser(int user_id, SqlConnection connection)
        {
            List<ClassRegistration> registrations = new List<ClassRegistration>();
            try
            {
                string query = @"SELECT * FROM ClassRegistration WHERE user_id = @user_id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClassRegistration registration = new ClassRegistration
                            {
                                Registration_id = Convert.ToInt32(reader["registration_id"]),
                                User_id = Convert.ToInt32(reader["user_id"]),
                                Class_session_id = Convert.ToInt32(reader["class_session_id"]),
                                Payment = Convert.ToBoolean(reader["payment"]),
                                Register_date = Convert.ToDateTime(reader["register_date"])
                            };
                            registrations.Add(registration);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                throw ex;
            }

            return registrations;
        }

        public Response UpdateClassRegistration(ClassRegistration registration, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"UPDATE ClassRegistration 
                             SET user_id = @user_id, class_session_id = @class_session_id, 
                                 payment = @Payment, register_date = @register_date
                             WHERE registration_id = @registration_id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@user_id", registration.User_id);
                    cmd.Parameters.AddWithValue("@class_session_id", registration.Class_session_id);
                    cmd.Parameters.AddWithValue("@Payment", registration.Payment);
                    cmd.Parameters.AddWithValue("@register_date", registration.Register_date);
                    cmd.Parameters.AddWithValue("@registration_id", registration.Registration_id);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Class registration updated.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Class registration update failed.";
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

        public Response DeleteClassRegistration(int registration_id, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                string query = @"DELETE FROM ClassRegistration WHERE registration_id = @registration_id";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@registration_id", registration_id);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200;
                        response.StatusMessage = "Class registration deleted.";
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Class registration deletion failed.";
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
        //visualization
        public List<Users> GetAllUsers(SqlConnection connection)
        {
            List<Users> users = new List<Users>();
            try
            {
                string query = @"SELECT ID, FirstName, LastName, Email, Type, Status, CreatedOn, IsApproved, PhoneNo, UserEmail 
                         FROM Users";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Users user = new Users
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Type = reader["Type"].ToString(),
                                CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                                PhoneNo = reader["PhoneNo"].ToString(),
                                UserEmail = reader["UserEmail"].ToString()
                            };
                            users.Add(user);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return users;
        }

    }
}
