﻿namespace Quik_BookingApp.BOs.Response
{
    public class WorkingSpaceResponseAmenities
    {
        public string SpaceId { get; set; }
        public string BusinessId { get; set; }
        public string ImageId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal PricePerHour { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
        public IFormFile Image { get; set; }

        // Add a list of amenities
        public List<AmenityRequestModel> Amenities { get; set; }
    }

    public class AmenityRequestModel
    {
        public string AmenityId { get; set; }
        public string AmenityText { get; set; }
    }
}