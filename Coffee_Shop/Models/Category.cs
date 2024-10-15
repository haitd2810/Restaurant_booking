using System;
using System.Collections.Generic;

namespace Coffee_Shop.Models
{
    public partial class Category
    {
        public Category()
        {
            Menus = new HashSet<Menu>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
