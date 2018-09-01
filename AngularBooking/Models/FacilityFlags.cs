using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Models
{
    [Flags]
    public enum FacilityFlags
    {
        Toilets = 1,
        Parking = 2,
        DisabledAccess = 4,
        AudioDescribed = 8,
        GuideDogsPermitted = 16,
        Subtitled = 32,
        Bar = 64
    }
}
