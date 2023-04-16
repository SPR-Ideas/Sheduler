using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace WebApp_Entity.Models

{
    public class UserModel
    {
        int id { get; set; }
        public string Name { get; set; }
        public string phone { get; set; }
        public string Password { get; set; }
        public string username { get; set; }

        public bool verifyUser() {
            string sqlConnectionString = "Data Source=localhost;Initial Catalog=AppointmentSheduler;Integrated Security=True;Encrypt=False";


            try {
                using (SqlConnection connection = new SqlConnection(sqlConnectionString)) {

                    SqlCommand cmd = new SqlCommand($"select * from UserTable where username='{this.username}' and password= '{this.Password}'"
                        , connection);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        this.id = reader.GetInt32(0);
                        this.Name = reader.GetString(1);
                        this.username = reader.GetString(2);
                        this.phone = reader.GetString(4);

                        // Erasing the password.
                        this.Password = "";
                        return true;
                    }
                }
            }

            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

    }


}
