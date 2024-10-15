using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.DAO;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Repos.Interface;

namespace Quik_BookingApp.Service
{
    public class ReviewService : IReviewService
    {
        private readonly QuikDbContext _context;

        public ReviewService(QuikDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(Guid reviewId)
        {
            return await _context.Reviews
                                 .Include(r => r.User)
                                 .Include(r => r.WorkingSpace)
                                 .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            review.ReviewId = Guid.NewGuid();
            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateReviewAsync(Review review)
        {
            var existingReview = await _context.Reviews.FindAsync(review.ReviewId);
            if (existingReview == null) return null;

            existingReview.Rating = review.Rating;
            existingReview.Comment = review.Comment;
            // Add other properties to update if needed

            await _context.SaveChangesAsync();
            return existingReview;
        }

        public async Task<bool> DeleteReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
