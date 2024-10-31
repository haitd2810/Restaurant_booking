using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Category
    {
        public Category()
        {
            Menus = new HashSet<Menu>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? DeleteFlag { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
