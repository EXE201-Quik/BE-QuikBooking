using AutoMapper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Models;


namespace Quik_BookingApp.Modal
{
    public class AutoMapperHandler : Profile
    {
        public AutoMapperHandler()
        {
            CreateMap<User, UserModal>().ForMember(item => item.Status, opt => opt.MapFrom(
                item => (item.IsActive != null && item.IsActive) ? "Active" : "In active")).ReverseMap();
        }
    }
}
