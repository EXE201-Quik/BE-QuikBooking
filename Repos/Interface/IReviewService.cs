using Quik_BookingApp.DAO.Models;

namespace Quik_BookingApp.Repos.Interface
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task<Review> CreateReviewAsync(Review review);
        Task<Review> UpdateReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(Guid reviewId);
    }

}
