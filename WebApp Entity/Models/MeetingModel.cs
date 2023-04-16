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
                    cmd.ExecuteNonQuery();
                    
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void deleteMeeting(int MeetId)
        {
            try
            {
                string sqlConnectionString = "Data Source=localhost;Initial Catalog=AppointmentSheduler;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {

                    SqlCommand cmd = new SqlCommand(
                        $"Delete meeting where id= {MeetId}"
                        , connection);
                    connection.Open();
                    cmd.ExecuteNonQuery ();
                    
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void makeAppointment(Meet meet)
        {
            try
            {
                string sqlConnectionString = "Data Source=localhost;Initial Catalog=AppointmentSheduler;Integrated Security=True;Encrypt=False";

                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {

                    SqlCommand cmd = new SqlCommand(
                        $"Insert Into Meeting(name, from_user,to_user,timing) values('{meet.Name}',{meet.from_user},{meet.to_user},'{meet.timing}' );"
                        , connection);
                    connection.Open();
                    int rows =cmd.ExecuteNonQuery();
                    if (rows > 0) { Console.WriteLine("Meeting Added"); }
                    else { Console.WriteLine("Meeting not Added"); }
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

    
}
