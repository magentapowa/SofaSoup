using System;
using System.Collections.Generic;
using System.Data.SqlClient;


using System.Configuration;


namespace SofaSoupApp
{
    public class DBManager
    {
        private readonly string ConnectionString;
        public SqlConnection Connection;

        //
        //Methods
        //
        public List<User> LoadUsers(){
            List<User> UL = new List<User>();
            using(SqlConnection conn = new SqlConnection(this.ConnectionString)){
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Server is under maintenance. Please try again later. Thank you!!",ex);
                }

                using (SqlCommand command = new SqlCommand("Select * from Users", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int UserID = (int)reader["UserID"];
                                string username = (string)reader["username"];
                                string password = (string)reader["password"];
                                LVL LVL = (LVL)Enum.Parse(typeof(LVL), reader["LVL"].ToString());
                                var MemberSince = reader["MemberSince"].ToString();

                                UL.Add(new User(UserID,username,password,LVL,DateTime.Parse(MemberSince)));
                            }
                        }
                    }

                }

            }
            return UL;
        }
        public List<Event> LoadSaves(User user, List<User> users)
        {
            List<Event> Saves = new List<Event>();
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string query =
                    "select * from Events " +
                    $"where EventID in (SELECT EventID from Saves WHERE UserID = {user.UserID})";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int eventID = (int)reader["EventID"];
                                string Description = (string)reader["Description"];
                                string Address = (string)reader["Address"];
                                DateTime date = DateTime.Parse(reader["Date"].ToString());
                                User temp;
                                if (string.IsNullOrEmpty(reader["UserID"].ToString()))
                                {
                                    temp = null;
                                }
                                else
                                {
                                    temp = users.Find(u => u.UserID == int.Parse(reader["UserID"].ToString()));
                                }

                                Saves.Add(new Event()
                                {
                                    EventID = eventID,
                                    Description = Description,
                                    Address = Address,
                                    Date = date,
                                    User = temp
                                });
                            }
                        }
                    }

                }

            }
            return Saves;
        }
        public List<Event> LoadSoup(List<User> users)
        {
            List<Event> soup = new List<Event>();
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand("Select * from Events", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int eventID = (int)reader["EventID"];
                                string Description = (string)reader["Description"];
                                string Address = (string)reader["Address"];
                                DateTime date = DateTime.Parse(reader["Date"].ToString());
                                User user;
                                if (string.IsNullOrEmpty(reader["UserID"].ToString()))
                                {
                                    user = null; 
                                }
                                else
                                {
                                    user = users.Find(u => u.UserID == int.Parse(reader["UserID"].ToString()));
                                }

                                soup.Add(new Event()
                                {
                                    EventID = eventID,
                                    Description = Description,
                                    Address = Address,
                                    Date = date,
                                    User = user

                                });
                            }
                        }
                    }

                }

            }
            return soup;
        }

        public int SaveNewUser(User newUser)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();

                // Write user to DB and get his Identity
                string query = 
                    "Insert into Users" +
                    "(username,password,LVL,memberSince)" +
                    "values(@name,@pass,@lvl,@date);" +
                    "SELECT CAST(scope_identity() AS int)";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.Add(new SqlParameter("@name", newUser.username));
                    command.Parameters.Add(new SqlParameter("@pass", newUser.password));
                    command.Parameters.Add(new SqlParameter("@lvl", 1));
                    command.Parameters.Add(new SqlParameter("@date", DateTime.Now));

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public void DropUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string query =
                    "DELETE FROM Users " +
                    $"WHERE [username] = '{user.username}'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdateUserValue<T>(User user, string columnName, T value)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string query = 
                    "UPDATE Users " +
                    "SET " +
                    $"[{columnName}] = '{value}' " +
                    $"WHERE [UserID]='{user.UserID}'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public int SaveNewEvent(User author, Event evnt)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();

                // Write event to DB and get its Identity
                string query =
                    "Insert into Events " +
                    "(Description,Address,Date,UserID) " +
                    "values(@desc,@address,@date,@userID); " +
                    "SELECT CAST(scope_identity() AS int)";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.Add(new SqlParameter("@desc", evnt.Description));
                    command.Parameters.Add(new SqlParameter("@address", evnt.Address));
                    command.Parameters.Add(new SqlParameter("@date", evnt.Date));
                    command.Parameters.Add(new SqlParameter("@userID", author.UserID));

                    return (int)command.ExecuteScalar();
                }
            }
        }
        public void DropEvent(Event evnt)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string query =
                    "DELETE FROM Events " +
                    $"WHERE [EventID] = '{evnt.EventID}'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddSave(User user, Event evnt)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();

                string query =
                    "Insert into Saves" +
                    "(UserID,EventID)" +
                    "values(@userID,@eventID);";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.Add(new SqlParameter("@userID", user.UserID));
                    command.Parameters.Add(new SqlParameter("@eventID", evnt.EventID));
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DropSave(User user, Event evnt)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string query =
                    "DELETE FROM Saves " +
                    $"WHERE [USerID] = '{user.UserID}' AND [EventID] = '{evnt.EventID}'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DropEventsCreatedBy(User user)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                string query = 
                    "DELETE FROM Events " +
                    $"WHERE [UserID] = '{user.UserID}'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Constractor
        public DBManager()
        {
            this.ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"];
        }
    }
}
