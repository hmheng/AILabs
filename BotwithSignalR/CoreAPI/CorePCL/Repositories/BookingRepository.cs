using CorePCL.Contexts;
using CorePCL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePCL.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private CoreDbContext dbContext;

        private DbSet<Booking> bookings;

        public BookingRepository(CoreDbContext context)
        {
            this.dbContext = context;
            bookings = dbContext.Set<Booking>();
        }

        public async Task<int> AddAsync(Booking booking)
        {
            if (booking == null)
            {
                throw new ArgumentNullException("booking");
            }

            await dbContext.AddAsync(booking);
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> AddMultipleAsync(IEnumerable<Booking> bookings)
        {
            if (bookings == null)
            {
                throw new ArgumentNullException("bookings");
            }

            foreach (var entity in bookings)
            {
                await dbContext.AddAsync(entity);
            }

            return await dbContext.SaveChangesAsync();
        }

        public IQueryable<Booking> GetAll()
        {
            return bookings.AsQueryable();
        }

    }
}
