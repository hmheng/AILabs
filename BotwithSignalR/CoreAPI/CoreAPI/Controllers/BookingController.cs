using CoreAPI.Entities;
using CoreAPI.Repositories;
using CoreAPI.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IHubContext<PushHub> _hubContext;
        private readonly IBookingRepository _bookingRepository;

        public BookingController(ILogger<BookingController> logger,
            IHubContext<PushHub> hubContext,
            IBookingRepository bookingRepository)
        {
            _logger = logger;
            _hubContext = hubContext;
            _bookingRepository = bookingRepository;
        }

        [HttpPost]
        public async Task CreateAsync(Booking booking)
        {
            try
            {
                if (booking.Id == null)
                {
                    booking.Id = Guid.NewGuid().ToString();
                    await _bookingRepository.AddAsync(booking);
                    await _hubContext.Clients.All.SendAsync("CreateProduct", booking);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        [HttpGet("get-all-bookings")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                return new OkObjectResult(await _bookingRepository.GetAll().OrderByDescending(x => x.CreatedAt).ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return BadRequest();
        }
    }
}
