using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary.Models
{
    public partial class Menu
    {
        public Menu()
        {
            BillInfors = new HashSet<BillInfor>();
        }

        public int Id { get; set; }
		public string? Name { get; set; }
		public string? Detail { get; set; }
		public double? Price { get; set; }
		public string? Img { get; set; }
		public int? CateId { get; set; }
        public bool? IsSell { get; set; }

        public virtual Category? Cate { get; set; }
        public virtual ICollection<BillInfor> BillInfors { get; set; }
    }
}
