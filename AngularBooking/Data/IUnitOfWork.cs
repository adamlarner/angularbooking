using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Data
{
    public interface IUnitOfWork
    {
        IRepository<Booking> Bookings { get; }
        IRepository<BookingItem> BookingItems { get; }
        IRepository<Customer> Customers { get; }
        IRepository<Event> Events { get; }
        IRepository<PricingStrategy> PricingStrategies { get; }
        IRepository<PricingStrategyItem> PricingStrategyItems { get; }
        IRepository<Room> Rooms { get; }
        IRepository<Showing> Showings { get; }
        IRepository<Venue> Venues { get; }
        IRepository<Feature> Features { get; }
    }
}
