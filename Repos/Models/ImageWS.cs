namespace Quik_BookingApp.Repos.Models
{
    public class ImageWS
    {
        public int ImageId { get; set; }
        public string WorkingSpaceName { get; set; }
        public string WSCode { get; set; }
        public byte[] WSImages { get; set; }

    }
}
