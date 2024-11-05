using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Feedback
    {
        public int Id { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Feedback1 { get; set; }
        public string? Img { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? FeedbackForDate { get; set; }
        public int? AccountId { get; set; }
        public string? Type { get; set; }
        public int? MenuId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Menu? Menu { get; set; }
    }
}
