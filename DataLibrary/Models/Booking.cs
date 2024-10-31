using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Booking
    {
        public int Id { get; set; }
        public int? TableId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Status { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? FullName { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual Table? Table { get; set; }
    }
}
