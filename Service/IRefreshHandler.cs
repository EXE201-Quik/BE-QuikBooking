namespace Quik_BookingApp.Service
{
    public interface IRefreshHandler
    {
       Task<string> GenerateToken(string username);
    }
}
