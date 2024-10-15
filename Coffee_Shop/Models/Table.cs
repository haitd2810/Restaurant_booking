using System;
using System.Collections.Generic;

namespace Coffee_Shop.Models
{
    public partial class Table
    {
        public Table()
        {
            Bills = new HashSet<Bill>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsOrder { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
