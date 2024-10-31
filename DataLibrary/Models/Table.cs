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
        public bool? IsOrder { get; set; }
        public bool? ForBooking { get; set; }
        public bool? DeleteFlag { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
