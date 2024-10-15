using System;
using System.Collections.Generic;

namespace Coffee_Shop.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillInfors = new HashSet<BillInfor>();
        }

        public int Id { get; set; }
        public int? TableId { get; set; }
        public bool? Payed { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? Status { get; set; }

        public virtual Table? Table { get; set; }
        public virtual ICollection<BillInfor> BillInfors { get; set; }
    }
}
