﻿using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.DAO;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Repos.Interface;
using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.BOs.Response;

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


        public async Task<List<ReviewResponseModel>> GetReviewsBySpaceIdAsync(string spaceId)
        {

            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.SpaceId == spaceId)
                .Select(r => new ReviewResponseModel
                {
                    ReviewId = r.ReviewId,
                    Username = r.Username,
                    SpaceId = r.SpaceId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return reviews;
        }



        public async Task<ReviewResponseModel> CreateReviewAsync(ReviewRequestModel reviewRequest)
            {
                var review = new Review
                {
                    ReviewId = Guid.NewGuid(),
                    Username = reviewRequest.Username,
                    SpaceId = reviewRequest.SpaceId,
                    Rating = reviewRequest.Rating,
                    Comment = reviewRequest.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return new ReviewResponseModel
                {
                    ReviewId = review.ReviewId,
                    Username = review.Username,
                    SpaceId = review.SpaceId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                };
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
