using AutoMapper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Repos.Response;


namespace Quik_BookingApp.Modal
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<User, UserModal>().ForMember(item => item.Status, opt => opt.MapFrom(
                item => (item.IsActive != null && item.IsActive) ? "Active" : "Inactive")).ReverseMap();
            CreateMap<WorkingSpace, WorkingSpaceModal>();
            CreateMap<Booking, BookingResponseModel>();
        }
    }
}
