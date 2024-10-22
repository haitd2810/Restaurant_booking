using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Token
    {
        public int Id { get; set; }
        public string? Token1 { get; set; }
        public DateTime? Date { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}
