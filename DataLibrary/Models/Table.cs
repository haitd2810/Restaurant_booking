using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
		[Required(ErrorMessage = "Table name is require")]
		public string? Name { get; set; }
        public bool? IsOrder { get; set; }
        public bool? Status { get; set; }
        public bool? ForBooking { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
