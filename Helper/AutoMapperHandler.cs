using AutoMapper;
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
            CreateMap<WorkingSpace, WorkingSpaceRequestModel>();
            CreateMap<Booking, BookingResponseModel>();
            CreateMap<Business, BusinessResponseModel>();
            CreateMap<UserRegister, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()) // Password will be hashed manually
                .ForMember(dest => dest.Role, opt => opt.Ignore());    // Default role will be set in the service
        }
    }
}
