namespace Quik_BookingApp.DAO.Models

{
    public class Business
    {
        public string BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string Presentor { get; set; } 
        public string Location { get; set; }
        public string Description { get; set; }
        
        public ICollection<WorkingSpace> WorkingSpaces { get; set; }
    }
}
