using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    public class PricingStrategyItem : IModel
    {
        public int Id { get; set; }
        public int PricingStrategyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; } 
        
        // navigation
        public PricingStrategy PricingStrategy { get; set; }
    }
}
