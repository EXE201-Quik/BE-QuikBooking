using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.BOs.Response;
using Quik_BookingApp.DAO;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Interface;

namespace Quik_BookingApp.Service
{
    public class BusinessService : IBusinessService
    {
        private readonly QuikDbContext _context;

        public BusinessService(QuikDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Business>> GetAllBusinessesAsync()
        {
            return await _context.Businesses.Include(b => b.WorkingSpaces).ToListAsync();
        }

        public async Task<Business> GetBusinessByIdAsync(string businessId)
        {
            return await _context.Businesses
                                 .Include(b => b.WorkingSpaces)
                                 .FirstOrDefaultAsync(b => b.BusinessId == businessId);
        }

        public async Task<Business> CreateBusinessAsync(Business business)
        {
            business.BusinessId = Guid.NewGuid().ToString();
            _context.Businesses.Add(business);
            await _context.SaveChangesAsync();
            return business;
        }

        public async Task<Business> UpdateBusinessAsync(Business business)
        {
            var existingBusiness = await _context.Businesses.FindAsync(business.BusinessId);
            if (existingBusiness == null) return null;

            existingBusiness.BusinessName = business.BusinessName;
            existingBusiness.PhoneNumber = business.PhoneNumber;
            existingBusiness.Email = business.Email;
            existingBusiness.Password = business.Password;
            existingBusiness.Location = business.Location;
            existingBusiness.Description = business.Description;

            await _context.SaveChangesAsync();
            return existingBusiness;
        }

        public async Task<bool> DeleteBusinessAsync(string businessId)
        {
            var business = await _context.Businesses.FindAsync(businessId);
            if (business == null) return false;

            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
