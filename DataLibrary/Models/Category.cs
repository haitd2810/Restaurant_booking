using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary.Models
{
    public partial class Category
    {
        public Category()
        {
            Menus = new HashSet<Menu>();
        }

        public int Id { get; set; }
		[Required(ErrorMessage = "Category name is require")]
		public string? Name { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
