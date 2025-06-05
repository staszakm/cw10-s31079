using System;
using System.Collections.Generic;

namespace cw10.Models;

public partial class Country
{
    public int IdCountry { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Trip> IdTrips { get; set; } = new List<Trip>();
    
    public virtual ICollection<CountryTrip> CountryTrips { get; set; } = new List<CountryTrip>();

}
