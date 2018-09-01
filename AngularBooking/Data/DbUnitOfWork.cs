using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularBooking.Models;

namespace AngularBooking.Data
{
    public class DbUnitOfWork : IUnitOfWork
    {
        public DbUnitOfWork(ApplicationDbContext context)
        {
            Bookings = new DbRepository<Booking>(context);
            BookingItems = new DbRepository<BookingItem>(context);
            Customers = new DbRepository<Customer>(context);
            Events = new DbRepository<Event>(context);
            PricingStrategies = new DbRepository<PricingStrategy>(context);
            PricingStrategyItems = new DbRepository<PricingStrategyItem>(context);
            Rooms = new DbRepository<Room>(context);
            Showings = new DbRepository<Showing>(context);
            Venues = new DbRepository<Venue>(context);
            Features = new DbRepository<Feature>(context);
        }

        public IRepository<Booking> Bookings { get; }
        public IRepository<BookingItem> BookingItems { get; }
        public IRepository<Customer> Customers { get; }
        public IRepository<Event> Events { get; }
        public IRepository<PricingStrategy> PricingStrategies { get; }
        public IRepository<PricingStrategyItem> PricingStrategyItems { get; }
        public IRepository<Room> Rooms { get; }
        public IRepository<Showing> Showings { get; }
        public IRepository<Venue> Venues { get; }
        public IRepository<Feature> Features { get; }
    }
}
