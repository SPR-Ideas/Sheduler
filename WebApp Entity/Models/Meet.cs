namespace WebApp_Entity.Models
{
    public class Meet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string from_user { get; set; }
        public string to_user { get; set; }
        public string timing { get; set; }
        public string confirmed { get; set; }

    }
}
