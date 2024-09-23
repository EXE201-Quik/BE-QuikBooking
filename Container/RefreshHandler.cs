using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Models;
using Quik_BookingApp.Service;
using System.Security.Cryptography;

namespace Quik_BookingApp.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly QuikDbContext context;
        public RefreshHandler(QuikDbContext context)
        {
            this.context = context;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var Existtoken = this.context.TblRefreshtokens.FirstOrDefaultAsync(item => item.UserId == username).Result;
                if (Existtoken != null)
                {
                    Existtoken.RefreshToken = refreshtoken;
                }
                else
                {
                    await this.context.TblRefreshtokens.AddAsync(new TblRefreshToken
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = refreshtoken
                    });
                }
                await this.context.SaveChangesAsync();

                return refreshtoken;

            }
        }
    }
}
