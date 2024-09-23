namespace Quik_BookingApp.Repos.Models
{
    public class Tempuser
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
    }
}
