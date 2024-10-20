using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class BillInfor
    {
        public int Id { get; set; }
        public int? BillId { get; set; }
        public int? MenuId { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public bool? Status { get; set; }

        public virtual Bill? Bill { get; set; }
        public virtual Menu? Menu { get; set; }
    }
}
