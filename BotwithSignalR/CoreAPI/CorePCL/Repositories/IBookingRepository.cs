using CorePCL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorePCL.Repositories
{
    public interface IBookingRepository
    {
        Task<int> AddAsync(Booking booking);
        Task<int> AddMultipleAsync(IEnumerable<Booking> bookings);
        IQueryable<Booking> GetAll();
    }
}
