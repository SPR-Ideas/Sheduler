using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WebApp_Entity.Models
{
    public class MeetingModel
    {
        public List<Meet> MeetingList= new List<Meet>();
        public void getMeetingList(int UserId) {
            try
            {
                string sqlConnectionString = "Data Source=localhost;Initial Catalog=AppointmentSheduler;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {

                    SqlCommand cmd = new SqlCommand(
                        $"select * from meeting where to_user={UserId} or from_user= {UserId}"
                        , connection);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Meet meet = new Meet();
                        meet.Id = reader.GetInt32(0);
                        meet.Name = reader.GetString(1);
                        meet.from_user = Convert.ToString( reader.GetInt32(2));
                        meet.to_user= Convert.ToString(reader.GetInt32(3));
                        meet.timing = reader.GetDateTime(4).ToString("d MMM h tt");
                        meet.confirmed = Convert.ToString(reader.GetBoolean(5));
                        MeetingList.Add(meet);
                    }
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);

            }
        }

        public void confirmMeeting(int MeetId) {
            try
            {
                string sqlConnectionString = "Data Source=localhost;Initial Catalog=AppointmentSheduler;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {

                    SqlCommand cmd = new SqlCommand(
                        $"Update meeting set confirmed = 1 where id= {MeetId}"
                        , connection);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Meet meet = new Meet();
                        meet.Id = reader.GetInt32(0);
                        meet.Name = reader.GetString(1);
                        meet.from_user = Convert.ToString(reader.GetInt32(2));
                        meet.to_user = Convert.ToString(reader.GetInt32(3));
                        meet.timing = reader.GetDateTime(4).ToString("d MMM");
                        meet.confirmed = Convert.ToString(reader.GetBoolean(5));
                        MeetingList.Add(meet);
                    }
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        
    }

    public class Meet {
        public int Id { get; set; }
        public string Name { get; set; }
        public string from_user { get; set; }
        public string to_user { get; set; }
        public string timing { get; set; }
        public string confirmed { get; set; }

    }
}
