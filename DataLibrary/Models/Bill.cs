using System;
using System.Collections.Generic;

namespace DataLibrary.Models
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
        public DateTime? CreateAt { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UpdateBy { get; set; }
        public bool? Status { get; set; }

        public virtual Table? Table { get; set; }
        public virtual ICollection<BillInfor> BillInfors { get; set; }
    }
}
