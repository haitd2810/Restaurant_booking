using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Ingredient
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Weight { get; set; }
        public double? Price { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
