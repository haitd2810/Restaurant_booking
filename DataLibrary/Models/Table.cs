using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Table
    {
        public Table()
        {
            Bills = new HashSet<Bill>();
            Bookings = new HashSet<Booking>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsOrder { get; set; }
        public bool? Status { get; set; }
        public bool? ForBooking { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
